using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PFSite.Models;
using PFSite.OAuth.GitHub;
using PFSite.Repositories;
using PFSite.Utils;
using System;
using System.Threading.Tasks;

namespace PFSite.Pages
{
    [TypeFilter(typeof(LocalUserFilter))]
    public class RecordModel : PageModel
    {
        public string Notification { get; set; }

        private readonly UserRepository _userRepo;
        private readonly RecordRepository _recordRepo;

        public RecordModel(
            UserRepository userRepo, RecordRepository recordRepo)
        {
            _userRepo = userRepo;
            _recordRepo = recordRepo;
        }

        public void OnGet()
        {
        }

        [Authorize]
        [Produces("application/json")]
        public async Task<JsonResult> OnGetMe()
        {
            string githubId = User.GetGitHubId();
            // 已由FIlter确保非空
            User user = await _userRepo.FindWithAsync(githubId);

            var result = await _recordRepo.ReportWithAsync(user.StudentId);

            return new JsonResult(result);
        }

        [Produces("application/json")]
        public async Task<JsonResult> OnGetAll()
        {
            var result = await _recordRepo.ReportAllAsync();

            return new JsonResult(result);
        }

        [Authorize]
        public async Task<IActionResult> OnPostSignIn()
        {
            string githubId = User.GetGitHubId();
            // 已由FIlter确保非空
            User user = await _userRepo.FindWithAsync(githubId);

            if (!await _userRepo.IsSignedAsync(user.StudentId))
            {
                // 在未签到情况下签到
                await _recordRepo.SignInAsync(user.StudentId, user.Name);

                Notification = "签到成功！" + DateTime.Now.ToLongTimeString();
            }
            else
            {
                ModelState.AddModelError("", "你已签到！");
            }

            return Page();
        }

        [Authorize]
        public async Task<IActionResult> OnPostSignOut()
        {
            string githubId = User.GetGitHubId();
            User user = await _userRepo.FindWithAsync(githubId);

            if (await _userRepo.IsSignedAsync(user.StudentId))
            {
                // 在已签到情况下签退
                await _recordRepo.SignOutAsync(user.StudentId);

                Notification = "签退成功！" + DateTime.Now.ToLongTimeString();
            }
            else
            {
                ModelState.AddModelError("", "你还未签到！");
            }

            return Page();
        }
    }
}
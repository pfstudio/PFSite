using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PFSite.Models;
using PFSite.OAuth.GitHub;
using PFSite.Repositories;
using System.Threading.Tasks;

namespace PFSite.Pages
{
    [Authorize]
    public class BindModel : PageModel
    {
        [BindProperty]
        public string StudentId { get; set; }
        [BindProperty]
        public string Name { get; set; }

        private readonly UserRepository _userRepo;

        public BindModel(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            string githubId = User.GetGitHubId();
            if (githubId == null)
            {
                ModelState.AddModelError("", "未能解析id");
                return Page();
            }

            User user = await _userRepo.FindWithAsync(githubId);
            if(user == null)
            {
                await _userRepo.BindUser(
                    new User
                    {
                        Name = Name,
                        StudentId = StudentId,
                        GitHubId = githubId
                    });

                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError("", "你已绑定信息！");
                return Page();
            }
        }
    }
}
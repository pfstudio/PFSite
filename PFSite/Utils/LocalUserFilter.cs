using Microsoft.AspNetCore.Mvc.Filters;
using PFSite.OAuth.GitHub;
using PFSite.Repositories;
using System.Threading.Tasks;

namespace PFSite.Utils
{
    /// <summary>
    /// 蒋没有绑定姓名学号的用户重定向至绑定页面
    /// </summary>
    public class LocalUserFilter : IAsyncResourceFilter
    {
        private UserRepository _userRepo;

        public LocalUserFilter(UserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            string githubId = context.HttpContext.User.GetGitHubId();

            if (context.HttpContext.User.Identity.IsAuthenticated
                && githubId != null && !await _userRepo.HasUserAsync(githubId))
            {
                context.HttpContext.Response.Redirect("/Bind");
            }

            await next.Invoke();
        }
    }
}

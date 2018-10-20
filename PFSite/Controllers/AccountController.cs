using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PFSite.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        /// <summary>
        /// 登录重定向
        /// </summary>
        /// <param name="returnUrl">重定向链接</param>
        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            if (!returnUrl.StartsWith('/'))
            {
                return LocalRedirect("/Index");
            }
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return LocalRedirect("/Index");
        }
    }
}
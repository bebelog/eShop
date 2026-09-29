using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace eShop.Web.Controllers
{
    [Route("/authenticate")]
    public class AuthenticationController : Controller
    {
        public async Task<IActionResult> Authenticate([FromQuery] string user, [FromQuery] string pwd)
        {
            if (user == "admin" && pwd == "adminadmin")
            {
                var userClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user),
                    new Claim(ClaimTypes.Email, "admin@eshop.com"),
                    new Claim(ClaimTypes.HomePhone, "12345678")
                };

                var userIdentify = new ClaimsIdentity(userClaims, "eShop.CookieAuth");
                var userPrincipal = new ClaimsPrincipal(userIdentify);

                await HttpContext.SignInAsync("eShop.CookieAuth", userPrincipal);

                return Redirect("/outstandingorders");
            }

            return Redirect("/");
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("eShop.CookieAuth");
            return Redirect("/");
        }
    }
}

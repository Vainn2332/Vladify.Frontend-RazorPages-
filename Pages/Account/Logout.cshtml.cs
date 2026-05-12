using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Vladify.Frontend.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            return ExecuteLogout();
        }

        public IActionResult OnPost()
        {
            return ExecuteLogout();
        }

        private IActionResult ExecuteLogout()
        {
            var redirectUrl = Url.Page("/Account/Login", new { errorType = "logout" });

            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri(redirectUrl!)
                .Build();

            return SignOut(
                authenticationProperties,
                CookieAuthenticationDefaults.AuthenticationScheme,
                Auth0Constants.AuthenticationScheme
            );
        }
    }
}

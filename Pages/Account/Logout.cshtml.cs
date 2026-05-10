using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Vladify.Frontend.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
           .WithRedirectUri("/")
           .Build();

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);

            return RedirectToPage("Account/Login");
        }
    }
}

using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Vladify.Frontend.Pages.Auth;

[AllowAnonymous] // Критически важно для страницы логина
public class LoginModel : PageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task OnPostAsync(string returnUrl = "/")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }
}
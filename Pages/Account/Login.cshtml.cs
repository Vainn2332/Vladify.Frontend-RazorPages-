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
        // Если пользователь уже авторизован, незачем ему видеть эту кнопку
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage("/Index");
        }
        return Page();
    }

    public async Task OnPostAsync(string returnUrl = "/")
    {
        // Создаем свойства аутентификации с URL перенаправления
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl)
            .Build();

        // Вызываем стандартный Challenge, который перекинет юзера на Auth0 Universal Login
        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }
}
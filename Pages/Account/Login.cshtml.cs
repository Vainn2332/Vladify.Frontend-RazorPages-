using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Vladify.Frontend.Pages.Auth;

[AllowAnonymous]
public class LoginModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? ErrorType { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }

    public string? ErrorMessage { get; private set; }
    public string? ErrorTitle { get; private set; }
    public string? ErrorIcon { get; private set; }
    public string ErrorStyle { get; private set; } = "error"; // "error" | "info" | "warning"

    public IActionResult OnGet()
    {
        switch (ErrorType)
        {
            case "email_not_verified":
                ErrorTitle = "Подтвердите вашу почту";
                ErrorMessage = string.IsNullOrEmpty(Email)
                    ? "Мы отправили письмо со ссылкой для подтверждения. Проверьте почту, перейдите по ссылке и затем попробуйте войти снова."
                    : $"Мы отправили письмо на {Email}. Перейдите по ссылке в письме и затем попробуйте войти снова.";
                ErrorIcon = "bi-envelope-check-fill";
                ErrorStyle = "warning";
                break;

            case "canceled":
                ErrorTitle = "Вход отменён";
                ErrorMessage = "Вы отменили вход. Попробуйте ещё раз, чтобы продолжить.";
                ErrorIcon = "bi-exclamation-triangle-fill";
                ErrorStyle = "error";
                break;

            case "logout":
                ErrorTitle = "Вы вышли из аккаунта";
                ErrorMessage = "До скорой встречи! Войдите снова, чтобы продолжить слушать музыку.";
                ErrorIcon = "bi-check-circle-fill";
                ErrorStyle = "info";
                break;

            case "access_denied":
                ErrorTitle = "Доступ запрещён";
                ErrorMessage = "Доступ к вашему аккаунту был запрещён. Проверьте права доступа.";
                ErrorIcon = "bi-shield-exclamation";
                ErrorStyle = "error";
                break;

            case "unknown":
                ErrorTitle = "Ошибка авторизации";
                ErrorMessage = "Что-то пошло не так. Попробуйте войти снова или свяжитесь с поддержкой.";
                ErrorIcon = "bi-x-circle-fill";
                ErrorStyle = "error";
                break;

            default:
                if (!string.IsNullOrEmpty(ErrorType))
                {
                    ErrorTitle = "Произошла ошибка";
                    ErrorMessage = "Не удалось выполнить вход. Пожалуйста, попробуйте ещё раз.";
                    ErrorIcon = "bi-exclamation-circle-fill";
                    ErrorStyle = "error";
                }
                break;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = "/")
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);

        return Page();
    }
}
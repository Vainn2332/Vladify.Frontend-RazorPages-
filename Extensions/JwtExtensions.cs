using System.Security.Claims;
using Vladify.Frontend.Exceptions;

namespace Vladify.Frontend.Extensions;

public static class JwtExtensions
{
    public static string GetEmail(this ClaimsPrincipal principal)
    {
        var email = principal.FindFirst("https://vladify.com/email")?.Value
            ?? throw new UnauthorizedException("unable to get user externalId");

        return email;
    }
}

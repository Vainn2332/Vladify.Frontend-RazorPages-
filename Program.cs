using Auth0.AspNetCore.Authentication;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Vladify.Frontend.Mappers;
using Vladify.Frontend.services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");

    options.Conventions.AllowAnonymousToPage("/Account/Login");
});

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0Options:Domain"]!;
    options.ClientId = builder.Configuration["Auth0Options:ClientID"]!;
    options.ClientSecret = builder.Configuration["Auth0Options:ClientSecret"];

    options.Scope = "openid profile email offline_access";

})
.WithAccessToken(options =>
{
    options.Audience = builder.Configuration["Auth0Options:ApiAudience"];

    options.UseRefreshTokens = true;
});
builder.Services.Configure<OpenIdConnectOptions>(Auth0Constants.AuthenticationScheme, options =>
{
    options.Events.OnRemoteFailure = context =>
    {
        var errorType = "unknown";

        if (context.Failure != null)
        {
            var msg = context.Failure.Message?.ToLower() ?? "";

            // Email не подтверждён
            if (msg.Contains("email") && (msg.Contains("verified") || msg.Contains("verify")))
            {
                errorType = "email_not_verified";
            }
            // Пользователь отменил вход
            else if (msg.Contains("access_denied"))
            {
                // Проверяем, не email_not_verified ли это (Auth0 часто шлёт access_denied с email-описанием)
                var queryError = context.Request.Query["error_description"].ToString().ToLower();

                if (queryError.Contains("email") && (queryError.Contains("verified") || queryError.Contains("verify")))
                {
                    errorType = "email_not_verified";
                }
                else
                {
                    errorType = "canceled";
                }
            }
        }

        // Также проверим query параметры напрямую (Auth0 шлёт их в редиректе)
        if (context.Request.Query.ContainsKey("error_description"))
        {
            var desc = context.Request.Query["error_description"].ToString().ToLower();
            if (desc.Contains("email") && (desc.Contains("verified") || desc.Contains("verify")))
            {
                errorType = "email_not_verified";
            }
        }

        // Передаём email если есть, чтобы показать в сообщении
        var email = context.Request.Query["email"].ToString();
        var redirectUrl = string.IsNullOrEmpty(email)
            ? $"/Account/Login?errorType={errorType}"
            : $"/Account/Login?errorType={errorType}&email={Uri.EscapeDataString(email)}";

        context.Response.Redirect(redirectUrl);
        context.HandleResponse();

        return Task.CompletedTask;
    };
});


builder.Services.AddAutoMapper(cfg => { }, typeof(UserMapper).Assembly);



builder.Services.AddHttpClient();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlaylistService>();
builder.Services.AddScoped<SongService>();
builder.Services.AddScoped<SearchService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

    mapper.ConfigurationProvider.AssertConfigurationIsValid();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages().WithStaticAssets();

app.Run();

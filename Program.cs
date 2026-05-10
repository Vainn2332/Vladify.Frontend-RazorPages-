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
        if (context.Failure != null && context.Failure.Message.Contains("access_denied"))
        {
            context.Response.Redirect("Account/Login?errorType=canceled");
            context.HandleResponse();
        }
        else
        {
            context.Response.Redirect("Account/Login?errorType=unknown");
            context.HandleResponse();
        }

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

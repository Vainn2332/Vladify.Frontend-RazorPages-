using Auth0.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");

    options.Conventions.AllowAnonymousToPage("/Auth/Login");
});

builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0Options:Domain"]!;
    options.ClientId = builder.Configuration["Auth0Options:ClientID"]!;
    options.ClientSecret = builder.Configuration["Auth0Options:ClientSecret"];
})
.WithAccessToken(options =>
{
    options.Audience = builder.Configuration["Auth0Options:ApiAudience"];
});

builder.Services.AddHttpClient();

var app = builder.Build();

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

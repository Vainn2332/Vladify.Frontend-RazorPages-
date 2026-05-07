using Auth0.AspNetCore.Authentication;
using AutoMapper;
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
})
.WithAccessToken(options =>
{
    options.Audience = builder.Configuration["Auth0Options:ApiAudience"];
});

builder.Services.AddAutoMapper(cfg => { }, typeof(UserMapper).Assembly);



builder.Services.AddHttpClient();

builder.Services.AddScoped<UserService>();

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

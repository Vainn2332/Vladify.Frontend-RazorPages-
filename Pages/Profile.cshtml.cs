using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly UserService _userService;
        private readonly HttpClient _httpClient;

        public ProfileModel(UserService userService, HttpClient httpClient)
        {
            _userService = userService;
            _httpClient = httpClient;
        }

        public UserModel UserModel { get; set; }

        public async Task OnGetAsync()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            UserModel = await _userService.GetCurrentUserAsync(accessToken);
        }
    }
}

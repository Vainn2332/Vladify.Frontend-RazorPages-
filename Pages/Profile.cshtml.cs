using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models.UserModels;
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

        [BindProperty]
        public UserUpdateRequestModel UserUpdateRequestModel { get; set; }
        public UserModel UserModel { get; set; }


        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            UserModel = await _userService.GetCurrentUserAsync(accessToken, cancellationToken);
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            UserModel = await _userService.UpdateUserAsync(UserModel.Id, UserUpdateRequestModel, accessToken, cancellationToken);

            return Page();
        }
    }
}

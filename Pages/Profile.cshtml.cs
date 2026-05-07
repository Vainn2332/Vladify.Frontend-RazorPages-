using AutoMapper;
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
        private readonly IMapper _mapper;

        public ProfileModel(UserService userService, HttpClient httpClient, IMapper mapper)
        {
            _userService = userService;
            _httpClient = httpClient;
            _mapper = mapper;
        }

        [BindProperty]
        public UserModel UserModel { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            UserModel = await _userService.GetCurrentUserAsync(accessToken!, cancellationToken);

        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var updateRequest = _mapper.Map<UserUpdateRequestModel>(UserModel);
            await _userService.UpdateUserAsync(UserModel.Id, updateRequest, accessToken!, cancellationToken);

            TempData["SuccessMessage"] = "Данные сохранены успешно!";

            return RedirectToPage("Index");
        }
    }
}

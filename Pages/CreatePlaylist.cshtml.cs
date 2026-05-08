using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models.PlaylistModels;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages
{
    public class CreatePlaylistModel(HttpClient client, PlaylistService playlistService) : PageModel
    {
        [BindProperty]
        public PlaylistAddRequestModel PlaylistAddRequestModel { get; set; }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            try
            {
                await playlistService.AddNewPlaylistAsync(PlaylistAddRequestModel, accessToken, cancellationToken);

                TempData["SuccessMessage"] = "Плейлист успешно создан!";

                return RedirectToPage("/Index");
            }
            catch
            {
                TempData["ErrorMessage"] = "что-то пошло не так";

                return Page();
            }
        }
    }
}

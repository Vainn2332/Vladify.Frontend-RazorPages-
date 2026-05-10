using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models;
using Vladify.Frontend.models.UserModels;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages
{
    public class MySongsModel(UserService userService, SongService songService) : PageModel
    {
        public required ICollection<SongModel> Songs { get; set; }
        public required UserModel Owner { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            Owner = await userService.GetCurrentUserAsync(accessToken!, cancellationToken);
            Songs = await songService.GetAllSongsOfUserAsync(Owner.Id, accessToken!, cancellationToken);
        }

        public async Task<IActionResult> OnPostDeleteSongAsync(Guid songId, CancellationToken cancellationToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            try
            {
                await songService.DeleteSongAsync(songId, accessToken!, cancellationToken);

                TempData["SuccessMessage"] = "Песня успешно удалена!";

                return RedirectToPage("MySongs");
            }
            catch
            {
                TempData["ErrorMessage"] = "что-то пошло не так";

                return RedirectToPage("MySongs");
            }
        }
    }
}

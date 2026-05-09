using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models.PlaylistModels;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages;

public class PlaylistsModel(PlaylistService playlistService) : PageModel
{
    [BindProperty]
    public PlaylistUpdateRequestModel PlaylistUpdateRequestModel { get; set; }

    public PlaylistModel PlaylistModel { get; set; }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        PlaylistModel = await playlistService.GetPlaylistByIdAsync(id, accessToken, cancellationToken);
    }

    public async Task<IActionResult> OnPostRemoveSongAsync(Guid id, Guid songId, CancellationToken cancellationToken)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        try
        {
            await playlistService.DeleteSongFromPlaylistAsync(id, songId, accessToken, cancellationToken);

            TempData["SuccessMessage"] = "Песня успешно удалена!";

            return RedirectToPage(new { id = id });
        }
        catch
        {
            TempData["ErrorMessage"] = "что-то пошло не так";

            return RedirectToPage(new { id = id });
        }
    }

    public async Task<IActionResult> OnPostUpdatePlaylistAsync(Guid id, CancellationToken cancellationToken)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        try
        {
            await playlistService.UpdatePlaylistAsync(PlaylistUpdateRequestModel, id, accessToken, cancellationToken);

            TempData["SuccessMessage"] = "Плейлист успешно обновлён!";

            return RedirectToPage(new { id = id });
        }
        catch
        {
            TempData["ErrorMessage"] = "что-то пошло не так";

            return RedirectToPage(new { id = id });
        }
    }

    public async Task<IActionResult> OnPostDeletePlaylistAsync(Guid id, CancellationToken cancellationToken)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        try
        {
            await playlistService.DeletePlaylistAsync(id, accessToken, cancellationToken);

            TempData["SuccessMessage"] = "Плейлист успешно удалён!";

            return RedirectToPage("Index");
        }
        catch
        {
            TempData["ErrorMessage"] = "что-то пошло не так";

            return RedirectToPage(new { id = id });
        }
    }

}

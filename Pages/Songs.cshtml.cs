using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models;
using Vladify.Frontend.models.PlaylistModels;
using Vladify.Frontend.models.UserModels;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages;

public class SongsModel(UserService userService, SongService songService, PlaylistService playlistService) : PageModel
{

    public required ICollection<SongModel> Songs { get; set; }
    public required UserModel Owner { get; set; }

    public required ICollection<PlaylistModel> UserPlaylists { get; set; }

    public async Task OnGetAsync(Guid userId, CancellationToken cancellationToken)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        Owner = await userService.GetUserByIdAsync(userId, accessToken!, cancellationToken);
        Songs = await songService.GetAllSongsOfUserAsync(Owner.Id, accessToken!, cancellationToken);

        var paginationFilter = new PaginationFilter(1, 100);
        var playlists = await playlistService.GetPlaylistsOfCurrentUserAsync(paginationFilter, accessToken!, cancellationToken);
        UserPlaylists = playlists.ToList();
    }

    public async Task<IActionResult> OnPostAsync(Guid userId, Guid songId, Guid playlistId, CancellationToken cancellationToken)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");


        try
        {
            await playlistService.AddSongToPlaylistAsync(songId, playlistId, accessToken, cancellationToken);

            TempData["SuccessMessage"] = "Песня успешно добавлена в плейлист!";

            return RedirectToPage(new { userId = userId });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;

            return RedirectToPage(new { userId = userId });
        }
    }
}

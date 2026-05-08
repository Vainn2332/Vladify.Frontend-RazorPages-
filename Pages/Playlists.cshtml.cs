using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models.PlaylistModels;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages;

public class PlaylistsModel(PlaylistService _playlistService) : PageModel
{
    public PlaylistModel PlaylistModel { get; set; }

    public async Task OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        PlaylistModel = await _playlistService.GetPlaylistByIdAsync(id, accessToken, cancellationToken);
    }
}

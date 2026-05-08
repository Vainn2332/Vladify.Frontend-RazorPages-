using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages;

public class PlaylistDetailsModel(PlaylistService _playlistService) : PageModel
{
    public PlaylistModel PlaylistModel { get; set; }

    public async Task OnGetAsync(Guid playlistId, CancellationToken cancellationToken)
    {
        var accessToken = await HttpContext.GetTokenAsync("access_token");

        PlaylistModel = await _playlistService.GetPlaylistByIdAsync(playlistId, accessToken, cancellationToken);
    }
}

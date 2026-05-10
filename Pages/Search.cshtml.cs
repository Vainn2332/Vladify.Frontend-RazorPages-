using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages
{
    public class SearchModel(SearchService searchService, PlaylistService playlistService) : PageModel
    {
        public async Task<IActionResult> OnGetResultsAsync(string q, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            {
                return new JsonResult(new { songs = Array.Empty<object>(), users = Array.Empty<object>() });
            }

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await searchService.SearchAsync(q, accessToken!, cancellationToken);

            var paginationFilter = new PaginationFilter(1, MyConstants.DropBoxPlaylistsPaginationPageSize);
            var playlists = await playlistService.GetPlaylistsOfCurrentUserAsync(paginationFilter, accessToken, cancellationToken);

            return new JsonResult(new
            {
                songs = result.Songs,
                users = result.Users,
                playlists
            });
        }

        public async Task<IActionResult> OnPostAddToPlaylistAsync(Guid songId, Guid playlistId, string returnUrl, CancellationToken cancellationToken)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            try
            {
                await playlistService.AddSongToPlaylistAsync(songId, playlistId, accessToken!, cancellationToken);
                TempData["SuccessMessage"] = "Песня успешно добавлена в плейлист!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return LocalRedirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
        }
    }
}

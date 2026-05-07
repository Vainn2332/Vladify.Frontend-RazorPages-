using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models;
using Vladify.Frontend.models.UserModels;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserService _userService;
        private readonly PlaylistService _playlistService;

        public IEnumerable<PlaylistModel> Playlists { get; set; } = [];
        public UserModel UserModel { get; set; }
        public IndexModel(UserService userService)
        {
            _userService = userService;
        }

        public async Task OnGetAsync([FromQuery] int pageNumber, CancellationToken cancellationToken, CancellationToken cancellationToken1)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            UserModel = await _userService.GetCurrentUserAsync(accessToken, cancellationToken);

            var paginationFilter = new PaginationFilter(pageNumber);
            Playlists = await _playlistService.GetPlaylistsOfUserAsync(UserModel.Id, paginationFilter, accessToken, cancellationToken);
        }
    }
}

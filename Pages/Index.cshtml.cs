using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models;
using Vladify.Frontend.models.PlaylistModels;
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
        public int CurrentPage { get; set; }


        public IndexModel(UserService userService, PlaylistService playlistService)
        {
            _userService = userService;
            _playlistService = playlistService;
        }

        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken, [FromQuery] int pageNumber = 1)
        {
            CurrentPage = pageNumber < 1 ? 1 : pageNumber;

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToPage("/Account/Login");
            }


            var paginationFilter = new PaginationFilter(CurrentPage);
            Playlists = await _playlistService.GetPlaylistsOfCurrentUserAsync(paginationFilter, accessToken, cancellationToken);

            return Page();
        }
    }
}

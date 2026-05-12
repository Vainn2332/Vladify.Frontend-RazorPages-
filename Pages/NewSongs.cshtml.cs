using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.models;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages
{
    public class NewSongsModel(SongService songService) : PageModel
    {
        public required ICollection<SongModel> Songs { get; set; }

        public async Task OnGetAsync(CancellationToken cancellationToken, int pageNumber = 1)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var paginationFilter = new PaginationFilter(pageNumber, MyConstants.NewSongsPaginationPageSize);

            Songs = await songService.GetRecentSongsAsync(paginationFilter, accessToken, cancellationToken);
        }
    }
}

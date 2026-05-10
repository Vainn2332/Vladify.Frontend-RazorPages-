using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages
{
    public class SearchModel(SearchService searchService) : PageModel
    {
        public async Task<IActionResult> OnGetResultsAsync(string q, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            {
                return new JsonResult(new { songs = Array.Empty<object>(), users = Array.Empty<object>() });
            }

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var result = await searchService.SearchAsync(q, accessToken!, cancellationToken);

            return new JsonResult(result);
        }
    }
}

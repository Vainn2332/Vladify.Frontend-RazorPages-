using Microsoft.AspNetCore.Mvc.RazorPages;
using Vladify.Frontend.services;

namespace Vladify.Frontend.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserService _userService;

        public IndexModel(UserService userService)
        {
            _userService = userService;
        }

        public string UserName { get; set; }

        public async Task OnGetAsync()
        {
            UserName = User.Identity.Name;
        }
    }
}

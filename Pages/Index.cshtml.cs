using BigTic.Data;
using BigTic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigTic.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SignInManager<Auth> _signInManager;
        private readonly UserManager<Auth> _userManager;
        private readonly BigTicContext _dbContext;
        private IGameService _gameService;

        public IndexModel(IGameService gameService, ILogger<IndexModel> logger, UserManager<Auth> userManager, SignInManager<Auth> signInManager, BigTicContext dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _gameService = gameService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.Name is not null && _gameService.Find(User.Identity.Name) is not null)
            {
                return RedirectToPage("/Game");
            }
            else
            {
                return Page();
            }
            if (User.Identity is not null && User.Identity.Name is not null)
            {
                return Page();
            }
            else
            {
                string user = DateTime.UtcNow.ToString().Replace(" ", "").Replace(".", "").Replace("-", "").Replace(":","") + "AA";
                while (await _userManager.FindByNameAsync(user) != null)
                {
                    user = DateTime.UtcNow.ToString();
                }
                var authUser = new Auth
                {
                    UserName = user
                };
                var result = await _userManager.CreateAsync(authUser, "P@ssw0rd1");
                var secondResult = await _signInManager.PasswordSignInAsync(user, "P@ssw0rd1", true, false);
                return Page();
            }
            
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var game = _gameService.Find("_null");
            if (game is not null)
            {
                game.User2 = User.Identity.Name;
                game.Game.User2 = User.Identity.Name;
                return RedirectToPage("/Game");
            }
            else
            {
                _gameService.Add(new Game(9), User.Identity.Name, "_null");
                game = _gameService.Find(User.Identity.Name);
                game.Game.User1 = User.Identity.Name;
                while (game.User2 == "_null") { }
                return RedirectToPage("/Game");
            }
        }
    }
}

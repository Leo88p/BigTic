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
        public bool findingGame = false;

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
            if (User.Identity.Name is not null && _gameService.Find(User.Identity.Name) is not null && _gameService.Find(User.Identity.Name).User2 != "_null")
            {
                return RedirectToPage("/Game");
            }
            else
            {
                if (_gameService.Find(User.Identity.Name) is not null && _gameService.Find(User.Identity.Name).User2 == "_null")
                {
                    findingGame = true;
                }
                else
                {
                    findingGame = false;
                }
                    return Page();
            }
        }
    }
}

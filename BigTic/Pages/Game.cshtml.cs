using BigTic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Drawing;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BigTic.Pages
{
    public class GameModel : PageModel
    {
        public Game? game;
        public int size;
        [BindProperty]
        public Coords Coords { get; set; } = new(null, null, null, null);
        public GameModel(IGameService gameService, UserManager<Auth> UserManager, IHttpContextAccessor HttpContextAccessor)
        {
            size = 9;
            var user = UserManager.GetUserName(HttpContextAccessor.HttpContext.User);
            if (user is not null)
            {
                var found = gameService.Find(user);
                if (found is not null)
                {
                    game = found.Game;
                }
                else
                {
                    game = null;
                }
            }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity is not null && User.Identity.Name is not null && game is not null && game.User2 != "_null")
            {
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }

    public record class Coords(double? x, double? y, double? r, double? w);
}

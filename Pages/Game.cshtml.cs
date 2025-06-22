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
        public Game game;
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
            if (User.Identity is not null && User.Identity.Name is not null && game is not null)
            {
                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
        public IActionResult OnPost()
        {
            if (Coords.x is null || Coords.y is null || Coords.r is null || Coords.w is null)
            {
                //return new JsonResult("x: " + Coords.x + " y: " + Coords.y + " r: " + Coords.r + " w: " + Coords.w);
                return new JsonResult(JsonSerializer.Serialize(game));
            }
            else
            {
                double x = (double)Coords.x;
                double y = (double)Coords.y;
                double w = (double)Coords.w;
                double r = (double)Coords.r;
                for (int i = 0; i < size + 1; i++)
                {
                    for (int j = 0; j < size + 1; j++)
                    {
                        List<Cell> adjs = [];
                        if (game.horizontalArcs[i][j].value != "empty" && game.PointLine(x, y, w * 0.025 + (i + 0.1) * r, w * 0.025 + j * r, w * 0.025 + (i + 0.9) * r, w * 0.025 + j * r, 5) && game.horizontalArcs[i][j].value == "0")
                        {
                            game.horizontalArcs[i][j].value = "1";
                            game.currentMove++;
                            adjs = game.horizontalArcs[i][j].adjesants;
                        }
                        if (game.verticalArcs[i][j].value != "empty" && game.PointLine(x, y, w * 0.025 + i * r, w * 0.025 + (j + 0.1) * r, w * 0.025 + i * r, w * 0.025 + (j + 0.9) * r, 5) && game.verticalArcs[i][j].value == "0")
                        {
                            game.verticalArcs[i][j].value = "1";
                            game.currentMove++;
                            adjs = game.verticalArcs[i][j].adjesants;
                        }
                        if (adjs.Count > 0)
                        {
                            int count = 0;
                            foreach (var adj in adjs)
                            {
                                int arcs = 0;
                                foreach (var a in adj.adjesants)
                                {
                                    if (a.value == "1")
                                    {
                                        arcs++;
                                    }
                                }
                                if (arcs == 4)
                                {
                                    adj.value = (game.player + 1).ToString();
                                    count++;
                                    game.score[game.player]++;
                                }
                            }
                            if (count == 0)
                            {
                                game.player = 1 - game.player;
                            }
                        }
                    }
                }
                return new JsonResult("x: " + x + " y: " + y + " r: " + r + " w: " + w);
            }
        }
    }

    public record class Coords(double? x, double? y, double? r, double? w);
}

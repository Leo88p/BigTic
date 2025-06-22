using BigTic.Models;
using BigTic.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Drawing;
using System.Text.Json;

namespace BigTic.Hubs
{
    public class GameHub : Hub
    {
        int size = 9;
        IGameService _gameService;
        UserManager<Auth> _userManager;
        public GameHub(IGameService gameService, UserManager<Auth> userManager)
        {
            _gameService = gameService;
            _userManager = userManager;
        }
        public async Task Send(string user, string X, string Y, string R, string W)
        {
            var gameRecord = _gameService.Find(user);
            var game = gameRecord.Game;
            var user1 = (await _userManager.FindByNameAsync(gameRecord.User1)).Id.ToString();
            var user2 = (await _userManager.FindByNameAsync(gameRecord.User2)).Id.ToString();

            if (X == "null" || Y == "null" || R == "null" || W == "null")
            {
                await Clients.User(user1).SendAsync("Recieve", JsonSerializer.Serialize(game));
                await Clients.User(user2).SendAsync("Recieve", JsonSerializer.Serialize(game));
            }
            else
            {
                double x = Convert.ToDouble(X);
                double y = Convert.ToDouble(Y);
                double r = Convert.ToDouble(R);
                double w = Convert.ToDouble(W);
                for (int i = 0; i < size + 1; i++)
                {
                    for (int j = 0; j < size + 1; j++)
                    {
                        List<Cell> adjs = [];
                        if (game.horizontalArcs[i][j].value != "empty" 
                            && game.PointLine(x, y, w * 0.025 + (i + 0.1) * r, w * 0.025 + j * r, w * 0.025 + (i + 0.9) * r, w * 0.025 + j * r, 5) 
                            && game.horizontalArcs[i][j].value == "0"
                            && (game.player==0 && user == gameRecord.User1 || game.player==1 && user== gameRecord.User2))
                        {
                            game.horizontalArcs[i][j].value = "1";
                            game.currentMove++;
                            adjs = game.horizontalArcs[i][j].adjesants;
                        }
                        else if (game.verticalArcs[i][j].value != "empty"
                            && game.PointLine(x, y, w * 0.025 + i * r, w * 0.025 + (j + 0.1) * r, w * 0.025 + i * r, w * 0.025 + (j + 0.9) * r, 5) 
                            && game.verticalArcs[i][j].value == "0"
                            && (game.player == 0 && user == gameRecord.User1 || game.player == 1 && user == gameRecord.User2))
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
                if (game.currentMove >= game.maxMoves)
                {
                    game.gameEnded = true;
                }
                await Clients.User(user1).SendAsync("Recieve", JsonSerializer.Serialize(game));
                await Clients.User(user2).SendAsync("Recieve", JsonSerializer.Serialize(game));
                if (game.gameEnded)
                {

                    _gameService.Remove(user);
                }
            }
        }
        public async Task Capitulate(string user)
        {
            var gameRecord = _gameService.Find(user);
            var game = gameRecord.Game;
            var user1 = (await _userManager.FindByNameAsync(gameRecord.User1)).Id.ToString();
            var user2 = (await _userManager.FindByNameAsync(gameRecord.User2)).Id.ToString();
            game.gameEnded = true;
            if (gameRecord.User1 == user)
            {
                game.playerSurrended = 1;
            } else
            {
                game.playerSurrended = 2;
            }
            await Clients.User(user1).SendAsync("Recieve", JsonSerializer.Serialize(game));
            await Clients.User(user2).SendAsync("Recieve", JsonSerializer.Serialize(game));
            _gameService.Remove(user);
        }
    }
}

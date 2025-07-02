using BigTic.Models;
using BigTic.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Drawing;
using System.Numerics;
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
                if (game.player == 0 && user == gameRecord.User1 || game.player == 1 && user == gameRecord.User2)
                {
                    game.CheckClick(x, y, r, w);
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

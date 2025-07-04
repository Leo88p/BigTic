using BigTic.Models;
using BigTic.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Drawing;
using System.Globalization;
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
        public async Task Send(string X, string Y, string R, string W)
        {
            string user = Context.User.Identity.Name;
            var gameRecord = _gameService.Find(user);
            var game = gameRecord.Game;
            var user1 = (await _userManager.FindByNameAsync(gameRecord.User1)).Id.ToString();
            var user2 = (await _userManager.FindByNameAsync(gameRecord.User2)).Id.ToString();
            Console.WriteLine($"Player name: {user}, User1 name: {gameRecord.User1}, User1 ID: {user1}, User2 name: {gameRecord.User2}, User2 ID: {user2}, X: {X}, Y:{Y}, R:{R}, W:{W}");

            double x = Double.Parse(X, CultureInfo.InvariantCulture);
            double y = Double.Parse(Y, CultureInfo.InvariantCulture);
            double r = Double.Parse(R, CultureInfo.InvariantCulture);
            double w = Double.Parse(W, CultureInfo.InvariantCulture);
            Console.WriteLine($"x: {x} y: {y} r: {r} w: {w}");
            Console.WriteLine($"GamePlayer: {game.player}");
            if (game.player == 0 && user == gameRecord.User1 || game.player == 1 && user == gameRecord.User2)
            {
                game.CheckClick(x, y, r, w);
                Console.WriteLine($"CheckClick was called");
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
        public async Task Capitulate()
        {
            string user = Context.User.Identity.Name;
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
        public async Task Init()
        {
            string user = Context.User.Identity.Name;
            var gameRecord = _gameService.Find(user);
            var game = gameRecord.Game;
            var user1 = (await _userManager.FindByNameAsync(gameRecord.User1)).Id.ToString();
            var user2 = (await _userManager.FindByNameAsync(gameRecord.User2)).Id.ToString();
            await Clients.User(user1).SendAsync("Recieve", JsonSerializer.Serialize(game));
            await Clients.User(user2).SendAsync("Recieve", JsonSerializer.Serialize(game));
        }
    }
}

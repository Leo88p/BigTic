using BigTic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace BigTic.Hubs
{
    public class JoinHub : Hub
    {
        IGameService _gameService;
        UserManager<Auth> _userManager;
        public JoinHub(IGameService gameService, UserManager<Auth> userManager)
        {
            _gameService = gameService;
            _userManager = userManager;
        }
        public async Task Join()
        {
            string user = Context.User.Identity.Name;
            var game = _gameService.Find("_null");
            if (game is not null)
            {
                game.User2 = user;
                game.Game.User2 = user;
                var user1 = (await _userManager.FindByNameAsync(game.User1)).Id.ToString();
                var user2 = (await _userManager.FindByNameAsync(game.User2)).Id.ToString();
                await Clients.User(user1).SendAsync("StartGame", JsonSerializer.Serialize(game));
                await Clients.User(user2).SendAsync("StartGame", JsonSerializer.Serialize(game));
            }
            else
            {
                _gameService.Add(new Game(9), user, "_null");
                game = _gameService.Find(user);
                game.Game.User1 = user;
                game.Game.User2 = "_null";
                var user1 = (await _userManager.FindByNameAsync(game.User1)).Id.ToString();
                await Clients.User(user1).SendAsync("WaitForGame", JsonSerializer.Serialize(game));
            }
        }
        public async Task Cancel()
        {
            string user = Context.User.Identity.Name;
            _gameService.Remove(user);
            var user1 = (await _userManager.FindByNameAsync(user)).Id.ToString();
            await Clients.User(user1).SendAsync("CancelledGame");
        }
    }
}

using BigTic.Data;
using BigTic.Hubs;
using BigTic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IGameService, GameService>();

builder.Services.AddDbContext<BigTicContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("BigTicContextConnection")));

builder.Services.AddIdentity<Auth, IdentityRole<long>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4;
})
 .AddEntityFrameworkStores<BigTicContext>()
 .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Index";
    options.LogoutPath = "/Logout";
    options.Cookie.Name = "BigTicAuth";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<BigTicContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapRazorPages();

app.MapHub<GameHub>("/move");

app.Run();

public class GameRecord
{
    public Game Game { get; set; }
    public string User1 { get; set; }
    public string User2 { get; set; }

    public GameRecord(Game game,  string user1, string user2)
    {
        this.Game = game;   
        this.User1 = user1;
        this.User2 = user2;
    }
}

public interface IGameService
{
    public void Add(Game game, string user1, string user2);
    public GameRecord? Find(string user);
    public void Remove(string user);
}

public class GameService : IGameService
{
    public List<GameRecord> Games { get; set; } = [];
    public void Add(Game game, string user1, string user2)
    {
        Games.Add(new GameRecord(game, user1, user2));
    }
    public GameRecord? Find(string user)
    {
        var g = Games.Where<GameRecord>(g => g.User1 == user || g.User2 == user).ToList();
        if (g.Count == 0)
        {
            return null;
        }
        else
        {
            return g[0];
        }
    }
    public void Remove(string user)
    {
        Games.RemoveAll(g => g.User1 == user || g.User2 == user);
    }
}

[Route("logout")]
public class LogoutController : ControllerBase
{
    private readonly SignInManager<Auth> _signInManager;
    public LogoutController(SignInManager<Auth> signInManager)
    {
        _signInManager = signInManager;
    }
    [HttpPost]
    public async Task<IActionResult> Logout(string path)
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Index");
    }
}
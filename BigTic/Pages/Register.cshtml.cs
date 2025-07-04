using BigTic.Data;
using BigTic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigTic.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Login { get; set; }
        [BindProperty]
        public string Password { get; set; }
        private readonly SignInManager<Auth> _signInManager;
        private readonly UserManager<Auth> _userManager;
        private readonly BigTicContext _dbContext;
        public RegisterModel(UserManager<Auth> userManager, SignInManager<Auth> signInManager, BigTicContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }
        public IActionResult OnGet()
        {
            if (User.Identity is not null && User.Identity.Name is not null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                return Page();
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Check if username or email already exists
            if (await _userManager.FindByNameAsync(Login) != null)
            {
                ModelState.AddModelError("Username", "Пользователь с этим именем уже существует");
                return Page();
            }



            // Now create the Auth entity with the same ID
            var authUser = new Auth
            {
                UserName = Login
            };

            // Create the user in Identity
            var result = await _userManager.CreateAsync(authUser, Password);
            if (result.Succeeded)
            {
                // First create the User entity
                await _dbContext.SaveChangesAsync(); // This generates the UserId

                await _signInManager.SignInAsync(authUser, isPersistent: true);

                return Redirect("/Index");
            }

            // If Identity creation fails, clean up the User entity we created
            //_dbContext.Users.Remove(userEntity);
            await _dbContext.SaveChangesAsync();

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return Page();
        }
    }
}

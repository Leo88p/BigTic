using BigTic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigTic.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Login { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public bool IsPersistant { get; set; }
        private readonly SignInManager<Auth> _signInManager;
        private readonly UserManager<Auth> _userManager;
        public LoginModel(UserManager<Auth> userManager, SignInManager<Auth> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            if (!ModelState.IsValid) return Page();

            var user = await _userManager.FindByNameAsync(Login);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, Password, IsPersistant, false);

                if (result.Succeeded)
                    return Redirect("/Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }
    }
}

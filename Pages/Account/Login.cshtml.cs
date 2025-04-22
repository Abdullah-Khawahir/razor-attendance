
public class LoginModel(SignInManager<User> signInManager) : PageModel
{
    private readonly SignInManager<User> _signInManager = signInManager;

    [BindProperty]
    public required InputModel Input { get; set; }

    public class InputModel
    {
        // [Required]
        // [EmailAddress]
        public required string Email { get; set; }

        // [Required]
        // [DataType(DataType.Password)]
        public required string Password { get; set; }
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, false);

        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }
}

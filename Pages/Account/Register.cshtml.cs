namespace razor.Pages;

public class RegisterModel : PageModel
{
    private readonly ILogger<RegisterModel> _logger;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    [BindProperty]
    public required RegisterFormModel Input { get; set; }

    public RegisterModel(ILogger<RegisterModel> logger, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _logger = logger;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnPost()
    {
        _logger.LogInformation("New Register Attempt - Email: {Email}, FullName: {FullName}", Input.Email, Input.FullName);
        var user = new User()
        {
            FullName = Input.FullName,
            Email = Input.Email,
            UserName = Input.Email,
            CreatedAt = DateTime.Now,
        };

        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, Input.Password);
        var result = await _userManager.CreateAsync(user);

        if (result.Succeeded)
        {
            _logger.LogInformation("New User Register  - Email: {Email}, FullName: {FullName} , Role: User", user.Email, user.FullName);

            const string roleName = "User";

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
            result = await _userManager.AddToRoleAsync(user, roleName);
        }

        if (result.Errors.Any() )
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return Page();
    }

    public class RegisterFormModel
    {
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}



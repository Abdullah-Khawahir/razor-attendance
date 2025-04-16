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
            if (_userManager.Users.Count() == 1)
            {
                _logger.LogInformation("New Admin Register  - Email: {Email}, FullName: {FullName} , Role: User", user.Email, user.FullName);
                var adminRole = "Admin";
                if (!await _roleManager.RoleExistsAsync(adminRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
                }
                result = await _userManager.AddToRoleAsync(user, "Admin");
            }
            else
            {
                _logger.LogInformation("New User Register  - Email: {Email}, FullName: {FullName} , Role: User", user.Email, user.FullName);
                var userRole = "User";
                if (!await _roleManager.RoleExistsAsync(userRole))
                {
                    await _roleManager.CreateAsync(new IdentityRole<Guid>(userRole));
                }
                result = await _userManager.AddToRoleAsync(user, userRole);
            }

        }

        if (result.Errors.Any())
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
        else return RedirectToPage("/Account/Login");
    }

    public class RegisterFormModel
    {
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}



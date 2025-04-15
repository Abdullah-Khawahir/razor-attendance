namespace razor.Pages;

public class LogoutModel : PageModel
{
    private readonly SignInManager<User> _signInManager;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(SignInManager<User> signInManager, ILogger<LogoutModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _signInManager.UserManager.GetUserAsync(User);
        var fullName = user?.FullName ?? "Unknown";
        _logger.LogInformation("User {FullName} logged out", fullName);
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Index");
    }

}

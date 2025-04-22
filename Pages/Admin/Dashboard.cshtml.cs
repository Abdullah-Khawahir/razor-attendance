using Microsoft.AspNetCore.Authorization;

namespace razor.Pages;

[Authorize(Roles = "Admin")]
public class DashboardModel(UserManager<User> userManager, ILogger<MailNotificationService> logger, RoleManager<IdentityRole<Guid>> roleManager, MailNotificationService mail) : PageModel
{
    [BindProperty]
    public List<UserWithRoleModel> Users { get; set; } = [];
    private readonly UserManager<User> _userManager = userManager;
    private readonly ILogger<MailNotificationService> _logger = logger;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager = roleManager;
    private readonly MailNotificationService _mail = mail;

    public async Task<IActionResult> OnGet()
    {
        var users = await _userManager.Users.ToListAsync();

        var usersWithRoles = await Task.WhenAll(users.Select(async user =>
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new UserWithRoleModel { User = user, Roles = [.. roles] };
        }));
        Users = [.. usersWithRoles];
        return Page();
    }

    public async Task<IActionResult> OnPostAdminToggleAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest();
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Admin"))
        {
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }
        else
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        return RedirectToPage();
    }
    public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
    {
        _logger.LogInformation($"attempt to delete user by Id {userId}");
        if (!Guid.TryParse(userId, out var guidUserId))
        {
            return RedirectToPage();
        }
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == guidUserId);
        if (user == null)
        {
            return RedirectToPage();
        }
        _mail.notifyUser(user, $"your account has been deleted");
        _logger.LogInformation($"deleted the user {user.FullName} with the email : {user.Email}");

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            _logger.LogError("delete faild: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return Page();
        }

        return RedirectToPage();

    }

    public class UserWithRoleModel
    {

        public required User User { get; set; }
        public required List<string> Roles { get; set; } = [];
        public bool IsAdmin => Roles.Contains("Admin");
    }
}

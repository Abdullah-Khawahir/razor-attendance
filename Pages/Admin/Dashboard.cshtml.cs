using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace razor.Pages;

[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    [BindProperty]
    public int counter { get; set; }
    [BindProperty]
    public List<UserWithRoleModel> Users { get; set; } = [];
    private readonly UserManager<User> _userManager;
    private readonly ILogger<MailNotificationService> _logger;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public DashboardModel(UserManager<User> userManager, ILogger<MailNotificationService> logger, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _logger = logger;
        _roleManager = roleManager;
    }

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
            if (!(await _roleManager.RoleExistsAsync("Admin")))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("Admin"));
            }
            await _userManager.AddToRoleAsync(user, "Admin");
        }

        return RedirectToPage();
    }

    public class UserWithRoleModel
    {

        public required User User { get; set; }
        public required List<string> Roles { get; set; } = [];
        public bool isAdmin => Roles.Contains("Admin");
    }
}

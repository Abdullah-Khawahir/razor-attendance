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


    public DashboardModel(UserManager<User> userManager)
    {
        _userManager = userManager;
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



    public class UserWithRoleModel
    {

        public required User User { get; set; }
        public required List<string> Roles { get; set; }
    }
}

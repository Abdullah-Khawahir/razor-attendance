using System.Threading.Tasks;

public class UserDetailsModel : PageModel
{
    private readonly UserManager<User> _userManager;

    public UserDetailsModel(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public Guid Id { get; set; }
    [BindProperty]
    public User User { get; set; }

    public async Task<IActionResult> OnGet(Guid id)
    {
        // Load user details using the id
        Id = id;
        User = await _userManager.FindByIdAsync(id.ToString());
    
        return User == null ? NotFound() : Page();
    }
}


namespace razor.Pages;

public class EditUserModel(ILogger<EditUserModel> logger, UserManager<User> userManager) : PageModel
{
    private readonly ILogger<EditUserModel> _logger = logger;
    private readonly UserManager<User> _userManager = userManager;
    [BindProperty(SupportsGet = true)]
    public Guid UserId { get; set; }
    [BindProperty]
    public required User AppUser { get; set; }
    [BindProperty]
    public required InputModel Input { get; set; }

    public async Task<IActionResult> OnGet(string userId)
    {
        _logger.LogInformation("editing user of Id : {ID}", userId);
        User? user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return NotFound();
        }
        AppUser = user;
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        User? user = await _userManager.FindByIdAsync(UserId.ToString());
        if (user == null)
        {
            return NotFound();
        }
        AppUser = user;

        if (AppUser == null)
        {
            return RedirectToPage("Admin/Dashboard");
        }

        if (!String.IsNullOrWhiteSpace(Input.Email)) AppUser.Email = Input.Email;
        if (!String.IsNullOrWhiteSpace(Input.FullName)) AppUser.FullName = Input.FullName;
        if (!String.IsNullOrWhiteSpace(Input.Password)) AppUser.PasswordHash = new PasswordHasher<User>().HashPassword(AppUser, Input.Password);

        _logger.LogInformation(
            "Edited user of Id : {ID} , name: FROM {FullName} TO {NewFullName} , Email: FROM {Email} TO {NewEmail}",
            UserId,
            AppUser?.FullName, Input.FullName,
            AppUser?.Email, Input.Email
        );
        return RedirectToPage("/Admin/Dashboard");
    }


    public class InputModel
    {
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string Password { get; set; }
    }
}

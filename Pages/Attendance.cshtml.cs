using Microsoft.AspNetCore.Authorization;

namespace razor.Pages;

[Authorize]
public class AttendanceModel : PageModel
{
    [BindProperty]
    public List<Attendance> UserAttendance { get; set; } = new();

    private readonly ILogger<AttendanceModel> _logger;
    private readonly AttendanceService _service;
    private readonly UserManager<User> _userManager;

    public AttendanceModel(
        ILogger<AttendanceModel> logger,
        AttendanceService service,
        UserManager<User> userManager)
    {
        _logger = logger;
        _service = service;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGet()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("User not found in OnGet.");
            return RedirectToPage("/Index");
        }

        UserAttendance = await _service.GetUserAttendanceAsync(user.Id);
        return Page();
    }

    public async Task<IActionResult> OnPostCheckIn()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("User not found in OnPostCheckIn.");
            return RedirectToPage("/Index");
        }

        try
        {
            await _service.CheckInAsync(user.Id, DateTime.Now);
            _logger.LogInformation("User {UserId} checked in at {Time}", user.Id, DateTime.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Check-in failed for user {UserId}", user.Id);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCheckOut()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("User not found in OnPostCheckOut.");
            return RedirectToPage("/Index");
        }

        try
        {
            await _service.CheckOutAsync(user.Id, DateTime.Now);
            _logger.LogInformation("User {UserId} checked out at {Time}", user.Id, DateTime.Now);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Check-out failed for user {UserId}", user.Id);
        }

        return RedirectToPage();
    }
}

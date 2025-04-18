using System.Threading.Tasks;

public class UserDetailsModel : PageModel
{
    private readonly UserManager<User> _userManager;
    private readonly AttendanceService _attendance;

    public UserDetailsModel(UserManager<User> userManager, AttendanceService attendance)
    {
        _userManager = userManager;
        _attendance = attendance;
    }

    public Guid Id { get; set; }
    [BindProperty]
    public User User { get; set; }
    public double weeklyHours = -1;
    public async Task<IActionResult> OnGet(Guid id)
    {
        Id = id;
        User = await _userManager.Users
            .Include(u => u.AttendanceRecords)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (User != null)
        {
            weeklyHours = await _attendance.CalculateWeeklyHoursAsync(User.Id, DateTime.Today);
        }
        return User == null ? NotFound() : Page();
    }
}


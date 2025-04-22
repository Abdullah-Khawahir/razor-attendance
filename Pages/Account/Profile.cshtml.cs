namespace razor.Pages;

public class ProfileModel(UserManager<User> userManager, AttendanceService attendance) : PageModel
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly AttendanceService _attendance = attendance;

    public ProfilePageModel UserDetails { get; set; }

    public async Task<IActionResult> OnGet()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return NotFound();
        }
        var id = currentUser.Id;

        UserDetails = new();
        User? user = await _userManager.Users
            .Include(u => u.AttendanceRecords)
            .FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        UserDetails.User = user;

        if (UserDetails.User != null)
        {
            var weeklyAttenendaces = await _attendance.CalculateWeeklyHoursAsync(UserDetails.User.Id, DateTime.Today);
            var hrs = weeklyAttenendaces
                .Select(h => h.TotalHours)
                .Sum();
            var mins = weeklyAttenendaces
                .Select(h => h.TotalMinutes)
                .Sum();
            UserDetails.WeeklyHours = String.Format("{0:00}:{1:00}", hrs, mins);
        }

        return UserDetails.User == null ? NotFound() : Page();
    }

    public class ProfilePageModel
    {
        [BindProperty]
        public User User { get; set; }
        public Guid Id { get; set; }
        public string WeeklyHours { get; set; } = "00:00";
    }
}

namespace razor.Services;

public class AttendanceService
{
    private static readonly TimeSpan LATE_ATTENDANCE_TIME = new(8, 30, 0);
    private static readonly TimeSpan EARLY_ATTENDANCE_TIME = new(15, 30, 0);

    private readonly DatabaseContext _db;
    private readonly MailNotificationService _mail;

    public AttendanceService(DatabaseContext dbContext, MailNotificationService mail)
    {
        _db = dbContext;
        _mail = mail;
    }

    public async Task<List<Attendance>> GetUserAttendanceAsync(Guid userId)
    {
        return await _db.Attendances
            .Where(a => a.UserId == userId)
            .Include(a => a.User)
            .ToListAsync();
    }

    public async Task CheckInAsync(Guid userId, DateTime checkInTime, string? note = null)
    {
        var today = DateTime.Today;

        var todayAttendance = await _db.Attendances
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today);

        if (todayAttendance == null)
        {
            var isLate = checkInTime.TimeOfDay > LATE_ATTENDANCE_TIME;

            todayAttendance = new Attendance
            {
                UserId = userId,
                Date = today,
                CheckInTime = checkInTime,
                CheckInNote = note,
                IsLate = isLate,
                User = await _db.Users.FirstAsync(u => u.Id == userId)
            };

            await _db.Attendances.AddAsync(todayAttendance);
            await _mail.notifyUserById(userId,
                    isLate ?
                    $"your check in is late for {today} at {checkInTime.TimeOfDay}" :
                    $"your check in is on time for {today} at {checkInTime.TimeOfDay}"
                    );
        }
        else if (todayAttendance.CheckInTime == null)
        {
            todayAttendance.CheckInTime = checkInTime;
            todayAttendance.CheckInNote = note;
            todayAttendance.IsLate = checkInTime.TimeOfDay > LATE_ATTENDANCE_TIME;
        }
        else
        {
            throw new Exception("Already checked in today.");
        }

        await _db.SaveChangesAsync();
    }

    public async Task CheckOutAsync(Guid userId, DateTime checkOutTime, string? note = null)
    {
        var today = DateTime.Today;

        var attendance = await _db.Attendances
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today) ?? throw new Exception("No check-in record found for today.");
        if (attendance.CheckOutTime != null)
        {
            throw new Exception("Already checked out today.");
        }

        attendance.CheckOutTime = checkOutTime;
        attendance.CheckOutNote = note;
        attendance.IsEarlyLeave = checkOutTime.TimeOfDay < EARLY_ATTENDANCE_TIME;

        await _mail.notifyUserById(userId,
                attendance.IsEarlyLeave ?
                $"your check out is early for {today} at {checkOutTime.TimeOfDay}" :
                $"your check out is on time for {today} at {checkOutTime.TimeOfDay}"
                );
        await _db.SaveChangesAsync();
    }

    public async Task<double> CalculateWeeklyHoursAsync(Guid userId, DateTime? weekStart)
    {
        var startOfWeek = weekStart ?? DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
        var endOfWeek = startOfWeek.AddDays(7);

        var weeklyAttendances = await _db.Attendances
            .Where(a => a.UserId == userId && a.Date >= weekStart && a.Date < endOfWeek)
            .ToListAsync();

        return weeklyAttendances
            .Where(a => a.CheckInTime.HasValue && a.CheckOutTime.HasValue)
            .Sum(a => (a.CheckOutTime.Value - a.CheckInTime.Value).TotalHours);
    }

}

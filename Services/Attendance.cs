namespace razor.Services;

public class AttendanceService
{
    private static readonly TimeSpan LATE_ATTENDANCE_TIME = new(8, 30, 0);
    private static readonly TimeSpan EARLY_ATTENDANCE_TIME = new(15, 30, 0);

    private readonly DatabaseContext _db;

    public AttendanceService(DatabaseContext dbContext)
    {
        _db = dbContext;
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

        var attendance = await _db.Attendances
            .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today);

        if (attendance == null)
        {
            attendance = new Attendance
            {
                UserId = userId,
                Date = today,
                CheckInTime = checkInTime,
                CheckInNote = note,
                IsLate = checkInTime.TimeOfDay > LATE_ATTENDANCE_TIME,
                User = await _db.Users.FirstAsync(u => u.Id == userId)
            };

            await _db.Attendances.AddAsync(attendance);
        }
        else if (attendance.CheckInTime == null)
        {
            attendance.CheckInTime = checkInTime;
            attendance.CheckInNote = note;
            attendance.IsLate = checkInTime.TimeOfDay > LATE_ATTENDANCE_TIME;
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

namespace razor.Entities;
public class User : IdentityUser<Guid>
{
    public required string FullName { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
}

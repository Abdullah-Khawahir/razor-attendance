namespace razor.Entities;
public class User : IdentityUser<Guid>
{
    public required string FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Attendance> AttendanceRecords { get; set; } = new List<Attendance>();
}

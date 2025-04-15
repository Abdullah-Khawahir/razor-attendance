using System.ComponentModel.DataAnnotations;

namespace razor.Entities;

public class Attendance
{
    public int Id { get; set; }
    public required Guid UserId { get; set; }

    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }

    public bool IsLate { get; set; }
    public bool IsEarlyLeave { get; set; }

    public string? CheckInNote { get; set; }
    public string? CheckOutNote { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }


    public User User { get; set; }
}

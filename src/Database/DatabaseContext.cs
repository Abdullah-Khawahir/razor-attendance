namespace razor.Database;

public class DatabaseContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<User> Users { get; set; }
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
       : base(options) { }
}


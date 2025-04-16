var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);

// Configure DbContext
var env = builder.Environment;
var services = builder.Services;
services.AddScoped<AttendanceService, AttendanceService>()
        .AddScoped<MailNotificationService, MailNotificationService>();
if (env.IsDevelopment())
{
    services.AddDbContext<DatabaseContext>(options =>
        options.UseSqlite("Data Source=dev.db"));
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));
}

// Add Identity
services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
            })
    .AddEntityFrameworkStores<DatabaseContext>();

// Add Razor Pages
services.AddRazorPages();

services.AddSession();

var app = builder.Build();

// Middleware
if (!env.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

app.Run();

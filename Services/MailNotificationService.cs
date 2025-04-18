using System.Threading.Tasks;

namespace razor.Services;

public class MailNotificationService
{
    private readonly ILogger<MailNotificationService> _logger;
    private readonly DatabaseContext _db;

    public MailNotificationService(ILogger<MailNotificationService> logger, DatabaseContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task notifyUserById(Guid userId, string? content = null)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user != null)
        {
            notifyUser(user, content);
        }

    }
    public void notifyUser(User user, string? content = null)
    {
        _logger.LogInformation("Send Mail To {Name} at {Mail}", user.FullName, user.Email);

        if (user != null)
        {
            _logger.LogInformation(
                    "Send Mail To {Name} at {Mail} with content: {content}",
                    user.FullName,
                    user.Email,
                    content ?? ""
                    );

        }
    }
}

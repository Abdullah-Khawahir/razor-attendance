namespace razor.Services;

public class MailNotificationService
{
    private readonly ILogger<MailNotificationService> _logger;

    public MailNotificationService(ILogger<MailNotificationService> logger)
    {
        _logger = logger;
    }

    public void notifyUser(User user)
    {
        _logger.LogInformation("Send Mail To {Name} at {Mail}", user.FullName, user.Email);
    }
}

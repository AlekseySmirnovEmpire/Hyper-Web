using Server.Data.Email;

namespace Server.Services;

public interface IEmailService
{
    public void PutNewMessageInQueue(
        string templateName,
        EmailPriority priority,
        string subject,
        string emailTo,
        object? bodyModel);

    public Task SendMessage();
}
using MailKit.Net.Smtp;
using MimeKit;
using Server.Core.Email;
using Server.Data;
using Server.Data.Email;

namespace Server.Services;

public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly ApplicationDbContext _dbContext;
    private readonly EmailConfiguration _emailConfig;
    private readonly EmailTemplatingService _emailTemplatingService;

    public SmtpEmailService(
        EmailConfiguration emailConfig,
        ILogger<SmtpEmailService> logger,
        ApplicationDbContext dbContext,
        EmailTemplatingService emailTemplatingService)
    {
        _emailConfig = emailConfig;
        _logger = logger;
        _dbContext = dbContext;
        _emailTemplatingService = emailTemplatingService;
    }

    public void PutNewMessageInQueue(
        string templateName, 
        EmailPriority priority, 
        string subject, 
        string emailTo,
        object? bodyModel)
    {
        try
        {
            var body = _emailTemplatingService.GenerateHtml(templateName, bodyModel).Result;
            _dbContext.EmailSendingQueue.Add(new EmailSendingQueue
            {
                Id = new Guid(),
                EmailTo = emailTo,
                Body = body,
                Subject = subject,
                Priority = priority,
                Type = templateName,
                CreatedAt = DateTime.Now
            });
            _dbContext.SaveChanges();
            _logger.LogInformation($"УСПЕХ при попытке положить письмо в очередь: тема - '{subject}', кому - '{emailTo}'!");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при попытке положить письмо в очередь: тема - '{subject}', кому - '{emailTo}'! Ошибка: {ex.Message}");
        }
    }

    public async Task SendMessage()
    {
        List<EmailSendingQueue> queue;
        try
        {
            queue = _dbContext.EmailSendingQueue
                .Where(esq => !esq.SentAt.HasValue && !string.IsNullOrEmpty(esq.Body))
                .OrderByDescending(esq => esq.Priority)
                .ThenBy(esq => esq.CreatedAt)
                .Take(5)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при выборе писем из очереди! Ошибка: {ex.Message}");
            return;
        }

        if (!queue.Any())
        {
            return;
        }

        using var client = new SmtpClient();
        try
        {
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
            await client.ConnectAsync(_emailConfig.MailServer, _emailConfig.MailPort, false);
            await client.AuthenticateAsync(_emailConfig.MailServerUserName, _emailConfig.MailServerPassword);

            foreach (var email in queue)
            {
                try
                {
                    var emailMessage = new MimeMessage();
                    emailMessage.From.Add(new MailboxAddress(string.Empty, _emailConfig.EmailFrom));
                    emailMessage.To.Add(new MailboxAddress(string.Empty, email.EmailTo));

                    emailMessage.Subject = email.Subject;
                    var builder = new BodyBuilder
                    {
                        HtmlBody = email.Body
                    };

                    emailMessage.Body = builder.ToMessageBody();
                    await client.SendAsync(emailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка при отправке письма для '{email.EmailTo}'! Ошибка: {ex.Message}");
                    email.ErrorLog = ex.Message;
                }

                email.SentAt = DateTime.Now;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка при отправке писем из очереди! Ошибка: {ex.Message}");
        }
        finally
        {
            if (client.IsConnected)
            {
                await client.DisconnectAsync(true);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
namespace Server.Services.Jobs;

public class EmailSendingJob : BaseJob
{
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailSendingJob> _logger;
    
    public EmailSendingJob(
        IEmailService emailService,
        ILogger<EmailSendingJob> logger) 
        : base(nameof(EmailSendingJob), "0/20 * * * * ?")
    {
        _emailService = emailService;
        _logger = logger;
    }

    protected override async Task Execute() => await _emailService.SendMessage();

    protected override void LogError(string exMessage) =>
        _logger.LogError($"Отправка сообщений из очереди остановлена, ошибка: {exMessage}");

    public override string ToString() => nameof(EmailSendingJob);
}
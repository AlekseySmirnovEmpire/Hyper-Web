namespace Server.Data.Email;

public class EmailSendingQueue
{
    public Guid Id { get; set; }

    public string Type { get; set; }

    public string Subject { get; set; }
    
    public string EmailTo { get; set; }
    
    public string Body { get; set; }
    
    public EmailPriority Priority { get; set; }
    
    public string? ErrorLog { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? SentAt { get; set; }
}
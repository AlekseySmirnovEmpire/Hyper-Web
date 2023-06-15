namespace Server.Core.Email;

public class EmailConfiguration
{
    public string MailServer { get; }
    
    public string MailServerUserName { get; }
    
    public string MailServerPassword { get; }
    
    public string EmailFrom { get; }
    
    public int MailPort { get; }

    public EmailConfiguration()
    {
        MailServer = Environment.GetEnvironmentVariable("ASPNETCORE_MailServer") ?? string.Empty;
        MailServerPassword = Environment.GetEnvironmentVariable("ASPNETCORE_MailServerPassword") ?? string.Empty;
        MailServerUserName = Environment.GetEnvironmentVariable("ASPNETCORE_MailServerUserName") ?? string.Empty;
        EmailFrom = Environment.GetEnvironmentVariable("ASPNETCORE_EmailFrom") ?? string.Empty;
        MailPort = Convert.ToInt32(Environment.GetEnvironmentVariable("ASPNETCORE_MailPort"));
    }
}
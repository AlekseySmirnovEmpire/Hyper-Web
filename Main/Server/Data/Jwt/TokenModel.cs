namespace Server.Data.Jwt;

public class TokenModel
{
    public string? Access { get; set; }
    
    public string? Refresh { get; set; }
}
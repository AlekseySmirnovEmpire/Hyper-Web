using Microsoft.AspNetCore.Mvc;

namespace Server.Services.Auth;

public class JwtAuthAttribute : TypeFilterAttribute
{
    public JwtAuthAttribute() : base(typeof(JwtAuthFilter))
    {
    }
}
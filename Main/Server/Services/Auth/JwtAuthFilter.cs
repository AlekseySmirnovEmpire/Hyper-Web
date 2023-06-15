using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Server.Services.Auth;

public class JwtAuthFilter : IAuthorizationFilter
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenService _tokenService;

    public JwtAuthFilter(IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
    {
        _httpContextAccessor = httpContextAccessor;
        _tokenService = tokenService;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer", string.Empty)
            .Trim();
        if (string.IsNullOrEmpty(token) || _tokenService.Validate(token) == null)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
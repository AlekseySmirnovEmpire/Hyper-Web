using Microsoft.AspNetCore.Mvc;
using Server.Data.Jwt;
using Server.Services;

namespace Server.Controllers;

[Route("/api/v1/token")]
public class TokenController : BaseApiController
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [Route("refresh")]
    [HttpPost]
    public IActionResult Refresh([FromBody] TokenModel? tokenModel)
    {
        try
        {
            return Ok(_tokenService.RefreshToken(tokenModel));
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                error = ex.Message
            });
        }
    }
}
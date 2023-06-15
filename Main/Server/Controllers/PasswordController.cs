using Microsoft.AspNetCore.Mvc;
using Server.Data.Dto;
using Server.Services;

namespace Server.Controllers;

[Route("/api/v1/password")]
public class PasswordController : BaseApiController
{
    private readonly PasswordService _passwordService;

    public PasswordController(PasswordService passwordService)
    {
        _passwordService = passwordService;
    }
    
    [Route("refresh-request")]
    [HttpPost]
    public IActionResult RefreshPasswordRequest([FromBody] LoginModelDto? loginModel)
    {
        try
        {
            _passwordService.RefreshPasswordRequest(loginModel);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                error = ex.Message
            });
        }
    }

    [Route("refresh/{userId:guid}")]
    [HttpPost]
    public IActionResult RefreshPassword([FromBody] LoginModelDto? loginModel, Guid userId)
    {
        try
        {
            _passwordService.RefreshPassword(userId, loginModel?.Password);
            return Ok();
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
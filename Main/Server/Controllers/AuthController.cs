using Microsoft.AspNetCore.Mvc;
using Server.Data.Dto;
using Server.Services;

namespace Server.Controllers;

[Route("/api/v1/auth")]
public class AuthController : BaseApiController
{
    private readonly UserService _userService;
    private readonly AuthService _authService;

    public AuthController(UserService userService, AuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [Route("login")]
    [HttpPost]
    public IActionResult Login([FromBody] LoginModelDto? loginModel)
    {
        try
        {
            return Ok(_authService.Login(loginModel));
        }
        catch (Exception ex)
        {
            return ex switch
            {
                UnauthorizedAccessException => Unauthorized(),
                _ => BadRequest(new
                {
                    error = ex.Message
                })
            };
        }
    }

    [Route("register")]
    [HttpPost]
    public IActionResult Register([FromBody] CreateUserDto? userModel)
    {
        try
        {
            return Ok(_userService.CreateUser(userModel));
        }
        catch (Exception ex)
        {
            return ex switch
            {
                InvalidDataException => StatusCode(StatusCodes.Status409Conflict, new
                {
                    my = true,
                    error = ex.Message
                }),
                _ => BadRequest(new
                {
                    my = false,
                    error = ex.Message
                })
            };
        }
    }

    [Route("logout")]
    [HttpPost]
    public IActionResult Logout()
    {
        try
        {
            _authService.Logout(HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer", string.Empty)
                .Trim());
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
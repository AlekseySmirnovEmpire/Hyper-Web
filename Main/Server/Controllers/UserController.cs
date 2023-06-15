using Microsoft.AspNetCore.Mvc;
using Server.Data.User;
using Server.Services;
using Server.Services.Auth;

namespace Server.Controllers;

[Route("/api/v1/users")]
public class UserController : BaseApiController
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [Route("find/{email}")]
    [HttpGet]
    [JwtAuth]
    public IActionResult Get(string email)
    {
        try
        {
            var user = _userService.GetUserByEmail(email);
            
            return user == null
                ? BadRequest("Пользователь не найден!")
                : !user.IsActive
                    ? BadRequest("Пользователь не активирован!")
                    : Ok(new UserResponseModel(user));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Route("confirm/{userId:guid}")]
    public IActionResult Confirm(Guid userId)
    {
        try
        {
            _userService.ConfirmUser(userId);
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
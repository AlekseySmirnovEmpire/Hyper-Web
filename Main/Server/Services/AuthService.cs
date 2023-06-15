using Server.Data.Dto;
using Server.Data.Jwt;

namespace Server.Services;

public class AuthService
{
    private readonly ITokenService _tokenService;
    private readonly UserService _userService;

    public AuthService(ITokenService tokenService, UserService userService)
    {
        _tokenService = tokenService;
        _userService = userService;
    }

    public TokenModel Login(LoginModelDto? loginModel)
    {
        if (loginModel == null || string.IsNullOrEmpty(loginModel.Email) || string.IsNullOrEmpty(loginModel.Password))
        {
            throw new Exception("Заполните данные!");
        }

        var user = _userService.GetUserByEmail(loginModel.Email);
        if (user == null)
        {
            throw new Exception("Неверно введены данные!");
        }

        if (!PasswordService.Validate(loginModel.Password, user.Password))
        {
            throw new Exception("Неверно введён пароль!");
        }

        if (!user.IsActive)
        {
            throw new Exception("Ваш аккаунт временно деактивирован или не был подтверждён!");
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var (refreshToken, expires) = _tokenService.GenerateRefreshToken(user);

        _userService.SetRefreshToken(user.Id, refreshToken, expires);

        return new TokenModel
        {
            Access = accessToken,
            Refresh = refreshToken
        };
    }

    public void Logout(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new Exception("Некорректный токен!");
        }

        var user = _tokenService.Validate(token);
        if (user == null)
        {
            throw new Exception("Некорректный токен!");
        }
        
        _userService.DropRefreshToken(user.Id);
    }
}
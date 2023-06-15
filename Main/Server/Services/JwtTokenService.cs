using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using Server.Data.Jwt;
using Server.Data.User;

namespace Server.Services;

public class JwtTokenService : ITokenService
{
    private readonly UserService _userService;

    public JwtTokenService(UserService userService)
    {
        _userService = userService;
    }

    public string GenerateAccessToken(UserModel userModel)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("JWT_ACCESS_SECRET") ?? string.Empty));
        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("SERVER_URL"),
            audience: Environment.GetEnvironmentVariable("SERVER_URL"),
            claims: new List<Claim>(),
            expires: DateTime.Now.AddSeconds(Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_ACCESS_EXPIRE"))),
            signingCredentials: signInCredentials)
        {
            Payload =
            {
                ["user"] = new TokenUserModel
                {
                    Email = userModel.Email,
                    Role = userModel.Role.RoleName,
                    UserName = userModel.UserName
                }
            }
        };

        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    public (string, DateTime) GenerateRefreshToken(UserModel userModel)
    {
        var secretKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("JWT_REFRESH_SECRET") ?? string.Empty));
        var signInCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.Now
            .AddSeconds(Convert.ToInt32(Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRE")));

        var tokenOptions = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("SERVER_URL"),
            audience: Environment.GetEnvironmentVariable("SERVER_URL"),
            claims: new List<Claim>(),
            expires: expires,
            signingCredentials: signInCredentials)
        {
            Payload =
            {
                ["user"] = new TokenUserModel
                {
                    Email = userModel.Email,
                    Role = userModel.Role.RoleName,
                    UserName = userModel.UserName
                }
            }
        };

        return (new JwtSecurityTokenHandler().WriteToken(tokenOptions), expires);
    }

    public UserModel? Validate(string token)
    {
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(
                token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            Environment.GetEnvironmentVariable("JWT_ACCESS_SECRET") ?? string.Empty)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                },
                out var validToken);

            if (validToken is not JwtSecurityToken jwtToken)
            {
                return null;
            }

            if (!jwtToken.Payload.TryGetValue("user", out var user) || user == null)
            {
                return null;
            }

            var tokenUserModel = JsonSerializer.Deserialize<TokenUserModel>(user.ToString() ?? string.Empty);

            var userModel = _userService.GetUserByEmail(tokenUserModel?.Email ?? string.Empty);
            return userModel is not { IsActive: true }
                ? null
                : userModel;
        }
        catch
        {
            return null;
        }
    }

    public TokenModel RefreshToken(TokenModel? tokenModel)
    {
        if (tokenModel == null || string.IsNullOrEmpty(tokenModel.Refresh))
        {
            throw new InvalidDataException("Остуствует тело запроса!");
        }

        var user = Validate(tokenModel.Refresh);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Рефреш токен некорректный!");
        }

        var accessToken = GenerateAccessToken(user);

        return new TokenModel
        {
            Access = accessToken,
            Refresh = user.RefreshToken
        };
    }

    private record TokenUserModel
    {
        [JsonPropertyName("Email")] public string? Email { get; set; }
        [JsonPropertyName("UserName")] public string? UserName { get; set; }
        [JsonPropertyName("Role")] public string? Role { get; set; }
    }
}
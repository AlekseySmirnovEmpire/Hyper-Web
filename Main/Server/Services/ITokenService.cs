using Server.Data.Jwt;
using Server.Data.User;

namespace Server.Services;

public interface ITokenService
{
    public string GenerateAccessToken(UserModel userModel);

    public (string, DateTime) GenerateRefreshToken(UserModel userModel);

    public UserModel? Validate(string token);

    public TokenModel RefreshToken(TokenModel? tokenModel);
}
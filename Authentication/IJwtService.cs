namespace ApiGateway.Authentication;

public interface IJwtService
{
    string CreateToken(int userId, DateTime expiresAt);

    int? VerifyTokenAndExtractUserId(string token);
}
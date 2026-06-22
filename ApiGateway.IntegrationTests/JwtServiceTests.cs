using ApiGateway.Authentication;
using Microsoft.Extensions.Options;

namespace ApiGateway.IntegrationTests;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        var options = Options.Create(new JwtOptions
        {
            SecretKey = "this-is-a-super-long-test-secret-key-for-jwt-tests",
            Issuer = "ApiGateway",
            Audience = "Waada_Clients"
        });

        _jwtService = new JwtService(options);
    }

    [Fact]
    public void CreateToken_WithValidUserId_ReturnsToken()
    {
        var token = _jwtService.CreateToken(
            userId: 42,
            expiresAt: DateTime.UtcNow.AddHours(1));

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void VerifyToken_WithValidToken_ReturnsUserId()
    {
        var token = _jwtService.CreateToken(
            userId: 42,
            expiresAt: DateTime.UtcNow.AddHours(1));

        var userId = _jwtService.VerifyTokenAndExtractUserId(token);

        Assert.Equal(42, userId);
    }

    [Fact]
    public void VerifyToken_WithInvalidToken_ReturnsNull()
    {
        var userId = _jwtService.VerifyTokenAndExtractUserId("not-a-real-token");

        Assert.Null(userId);
    }

    [Fact]
    public void VerifyToken_WithExpiredToken_ReturnsNull()
    {
        var token = _jwtService.CreateToken(
            userId: 42,
            expiresAt: DateTime.UtcNow.AddMinutes(-1));

        var userId = _jwtService.VerifyTokenAndExtractUserId(token);

        Assert.Null(userId);
    }
}
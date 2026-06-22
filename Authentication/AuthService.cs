using UserManagement.Grpc.Contracts;
using ApiGateway.Models.Auth;
using System.Security.Cryptography;
using System.Text;
using ApiGateway.Models.Auth;

namespace ApiGateway.Authentication;

public class AuthService : IAuthService
{
    private readonly IUserGrpcService _userGrpcService;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserGrpcService userGrpcService,
        IJwtService jwtService)
    {
        _userGrpcService = userGrpcService;
        _jwtService = jwtService;
    }

    public async Task<string> LoginAsync(LoginRequest request)
    {
        var userResponse = await _userGrpcService.GetUserAsync(new GetUserRequest
        {
            Email = request.Email
        });

        if (!userResponse.Success || userResponse.Data is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var user = userResponse.Data;

        var passwordHash = ComputeMd5Hash(request.Password + user.Salt);

        if (passwordHash != user.PasswordHash)
            throw new UnauthorizedAccessException("Invalid email or password.");

        var token = _jwtService.CreateToken(
            user.Id,
            DateTime.UtcNow.AddHours(1));
        
        var deviceResponse = await _userGrpcService.UpsertDeviceAsync(
            new UpsertDeviceRequest
            {
                UserDevice = new UserDeviceGrpcModel
                {
                    UserId = user.Id,
                    DeviceName = request.DeviceName,
                    DeviceType = request.DeviceType,
                    DeviceToken = request.DeviceToken,
                    RegisteredAt = DateTime.UtcNow,
                    IsActive = true
                }
            });
        
        if (!deviceResponse.Success || deviceResponse.Data is null)
            throw new Exception("Failed to register device.");

        var sessionResponse = await _userGrpcService.UpsertSessionAsync(
            new UpsertSessionRequest
            {
                UserSession = new UserSessionGrpcModel
                {
                    UserDeviceId = deviceResponse.Data.Id,
                    SessionToken = token,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(1),
                    IsActive = true
                }
            });

        if (!sessionResponse.Success)
            throw new Exception("Failed to create session.");
        
        return token;
    }

    private static string ComputeMd5Hash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = MD5.HashData(bytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
    
    public async Task<UserGrpcModel> ChangeCountryAsync(
        string token,
        ChangeCountryRequest request)
    {
        var userId = _jwtService.VerifyTokenAndExtractUserId(token);

        if (!userId.HasValue)
            throw new UnauthorizedAccessException("Invalid token.");

        var userResponse = await _userGrpcService.GetUserAsync(
            new GetUserRequest
            {
                UserId = userId.Value.ToString()
            });

        if (!userResponse.Success || userResponse.Data is null)
        {
            throw new Exception("User not found.");
        }

        var user = userResponse.Data;
        
        if (string.Equals(user.Country, request.Country, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("New country cannot be the same as current country.");
        }

        user.Country = request.Country;
        user.CountryCode = request.CountryCode;

        var updateResponse = await _userGrpcService.UpsertUserAsync(
            new UpsertUserRequest
            {
                User = user
            });

        if (!updateResponse.Success || updateResponse.Data is null)
        {
            throw new Exception("Failed to update country.");
        }

        return updateResponse.Data;
    }
}
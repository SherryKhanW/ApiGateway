using UserManagement.Grpc.Contracts;
using ApiGateway.Models.Auth;
using System.Security.Cryptography;
using System.Text;
using ApiGateway.Authentication.Models;
using NotificationService.Grpc.Contracts;
using StackExchange.Redis;

namespace ApiGateway.Authentication;

public class AuthService : IAuthService
{
    private readonly IUserGrpcService _userGrpcService;
    private readonly IJwtService _jwtService;
    private readonly INotificationGrpcService _notificationGrpcService;
    private readonly IDatabase _redis;

    public AuthService(
        IUserGrpcService userGrpcService,
        IJwtService jwtService,
        INotificationGrpcService notificationGrpcService,
        IConnectionMultiplexer redis)
    {
        _userGrpcService = userGrpcService;
        _jwtService = jwtService;
        _notificationGrpcService = notificationGrpcService;
        _redis = redis.GetDatabase();
    }
    
    public async Task<bool> RequestOtpAsync(RequestOtpRequest request)
    {
        var otp = Random.Shared.Next(100000, 999999).ToString();
        Console.WriteLine($"OTP for {request.Email}: {otp}");
        await _redis.StringSetAsync(

            $"otp:{request.Email}",

            otp,

            TimeSpan.FromMinutes(1));
        
        var response = await _notificationGrpcService.SendOtpEmailAsync(
            new SendOtpEmailRequest
            {
                Email = request.Email,
                Otp = otp
            });

        if (!response.Success)
        {
            throw new Exception(response.ErrorMessage);
        }
        
        return true;
    }
    
    public async Task<string> LoginAsync(LoginRequest request)
    {   
        var storedOtp = await _redis.StringGetAsync($"otp:{request.Email}");

        if (storedOtp.IsNullOrEmpty)
            throw new Exception("OTP expired.");

        if (storedOtp != request.Otp)
            throw new Exception("Invalid OTP.");
        
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
        
        await _redis.KeyDeleteAsync($"otp:{request.Email}");
        
        return token;
    }

    private static string ComputeMd5Hash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = MD5.HashData(bytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
    
    public async Task<UserGrpcModel> ChangeCountryAsync(
        int userId,
        ChangeCountryRequest request)
    {
        var userResponse = await _userGrpcService.GetUserAsync(
            new GetUserRequest
            {
                UserId = userId.ToString()
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
    
    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var userResponse = await _userGrpcService.GetUserAsync(new GetUserRequest
        {
            UserId = userId.ToString()
        });

        if (!userResponse.Success || userResponse.Data == null)
            throw new Exception("User not found.");

        var user = userResponse.Data;

        var oldPasswordHash = ComputeMd5Hash(request.OldPassword + user.Salt);

        if (oldPasswordHash != user.PasswordHash)
            return false;

        var newSalt = Guid.NewGuid().ToString("N");
        var newPasswordHash = ComputeMd5Hash(request.NewPassword + newSalt);

        user.Salt = newSalt;
        user.PasswordHash = newPasswordHash;

        var updateResponse = await _userGrpcService.UpsertUserAsync(new UpsertUserRequest
        {
            User = user
        });

        if (!updateResponse.Success)
            throw new Exception(updateResponse.ErrorMessage);

        return true;
    }
}
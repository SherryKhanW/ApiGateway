using ApiGateway.Models.Auth;
using UserManagement.Grpc.Contracts;
using ApiGateway.Authentication.Models;

namespace ApiGateway.Authentication;

public interface IAuthService
{
    Task<string> LoginAsync(LoginRequest request);
    Task<UserGrpcModel> ChangeCountryAsync(
        int token,
        ChangeCountryRequest request);
    
    Task<bool> ChangePasswordAsync(
        int userId,
        ChangePasswordRequest request);
    
    Task<bool> RequestOtpAsync(RequestOtpRequest request);
}
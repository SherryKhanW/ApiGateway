using ApiGateway.Models.Auth;
using UserManagement.Grpc.Contracts;
namespace ApiGateway.Authentication;

public interface IAuthService
{
    Task<string> LoginAsync(LoginRequest request);
    Task<UserGrpcModel> ChangeCountryAsync(
        string token,
        ChangeCountryRequest request);
}
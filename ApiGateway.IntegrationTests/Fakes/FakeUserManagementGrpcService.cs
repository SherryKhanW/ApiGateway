using UserManagement.Grpc.Contracts;

namespace ApiGateway.IntegrationTests.Fakes;

public class FakeUserManagementGrpcService : IUserGrpcService
{
    public Task<GetUserResponse> GetUserAsync(GetUserRequest request)
    {
        return Task.FromResult(new GetUserResponse
        {
            Success = request.Email == "test@example.com",
            ErrorMessage = request.Email == "test@example.com" ? null : "User not found",
            Data = request.Email == "test@example.com"
                ? new UserGrpcModel
                {
                    PublicId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com",
                    IsActive = true,
                    Country = "Pakistan",
                    CountryCode = "PK"
                }
                : null
        });
    }

    public Task<UpsertUserResponse> UpsertUserAsync(UpsertUserRequest request)
    {
        return Task.FromResult(new UpsertUserResponse
        {
            Success = true,
            Data = request.User
        });
    }

    public Task<UpsertDeviceResponse> UpsertDeviceAsync(UpsertDeviceRequest request)
    {
        return Task.FromResult(new UpsertDeviceResponse
        {
            Success = true,
            Data = request.UserDevice
        });
    }

    public Task<UpsertSessionResponse> UpsertSessionAsync(UpsertSessionRequest request)
    {
        return Task.FromResult(new UpsertSessionResponse
        {
            Success = true,
            Data = request.UserSession
        });
    }
}
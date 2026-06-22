using UserManagement.Grpc.Contracts;

namespace ApiGateway.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/users", async (
            string? email,
            string? userId,
            IUserGrpcService userGrpcService) =>
        {
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(userId))
            {
                return Results.BadRequest("Either email or userId is required.");
            }

            var response = await userGrpcService.GetUserAsync(new GetUserRequest
            {
                Email = email,
                UserId = userId
            });

            if (!response.Success)
            {
                return Results.NotFound(response);
            }

            return Results.Ok(response);
        });
        
        app.MapPost("/users", async (
            UpsertUserRequest request,
            IUserGrpcService userGrpcService) =>
        {
            var response = await userGrpcService.UpsertUserAsync(request);

            return response.Success
                ? Results.Ok(response)
                : Results.BadRequest(response);
        });
    }
}
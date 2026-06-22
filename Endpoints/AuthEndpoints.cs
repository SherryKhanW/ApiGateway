using ApiGateway.Authentication;
using ApiGateway.Models.Auth;

namespace ApiGateway.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/login", async (
            LoginRequest request,
            IAuthService authService) =>
        {
            try
            {
                var token = await authService.LoginAsync(request);

                return Results.Ok(new LoginResponse
                {
                    Token = token
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Unauthorized();
            }
        });
        
        app.MapGet("/auth/verify", (
            HttpRequest request,
            IJwtService jwtService) =>
        {
            var authorizationHeader = request.Headers.Authorization.ToString();

            if (string.IsNullOrWhiteSpace(authorizationHeader) ||
                !authorizationHeader.StartsWith("Bearer "))
            {
                return Results.Ok(false);
            }

            var token = authorizationHeader["Bearer ".Length..];

            var userId = jwtService.VerifyTokenAndExtractUserId(token);

            return Results.Ok(userId.HasValue);
        });
        
        app.MapPut("/auth/country",
            async (
                HttpContext httpContext,
                ChangeCountryRequest request,
                IAuthService authService) =>
            {
                var authorization = httpContext.Request.Headers.Authorization.ToString();

                if (!authorization.StartsWith("Bearer "))
                {
                    return Results.Unauthorized();
                }

                var token = authorization["Bearer ".Length..];

                try
                {
                    var user = await authService.ChangeCountryAsync(token, request);
                    return Results.Ok(user);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
                catch (InvalidOperationException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });
    }
}
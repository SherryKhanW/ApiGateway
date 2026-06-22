using ApiGateway.IntegrationTests.Fakes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UserManagement.Grpc.Contracts;

namespace ApiGateway.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the production registration
            services.RemoveAll<IUserGrpcService>();

            // Replace it with our fake
            services.AddSingleton<IUserGrpcService, FakeUserManagementGrpcService>();
        });
    }
}
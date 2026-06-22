using Grpc.Net.Client;
using ProtoBuf.Grpc.Client;
using UserManagement.Grpc.Contracts;

namespace ApiGateway.Grpc.Clients;

public static class GrpcClientServiceCollectionExtensions
{
    public static IServiceCollection AddGrpcClients(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var userManagementGrpcUrl =
            configuration["GrpcServices:UserManagement"]
            ?? "http://localhost:5235";

        services.AddSingleton<IUserGrpcService>(_ =>
        {
            var channel = GrpcChannel.ForAddress(userManagementGrpcUrl);
            return channel.CreateGrpcService<IUserGrpcService>();
        });

        return services;
    }
}
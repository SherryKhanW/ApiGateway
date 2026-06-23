using ApiGateway.Grpc.Notification;
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
        // UserManagement
        var userManagementGrpcUrl =
            configuration["GrpcServices:UserManagement"]
            ?? "http://localhost:5235";

        services.AddSingleton<IUserGrpcService>(_ =>
        {
            var channel = GrpcChannel.ForAddress(userManagementGrpcUrl);
            return channel.CreateGrpcService<IUserGrpcService>();
        });

        // NotificationService
        var notificationGrpcUrl =
            configuration["GrpcServices:NotificationService"]
            ?? "http://localhost:5111";

        services.AddSingleton<INotificationGrpcService>(_ =>
        {
            var channel = GrpcChannel.ForAddress(notificationGrpcUrl);
            return channel.CreateGrpcService<INotificationGrpcService>();
        });

        return services;
    }
}
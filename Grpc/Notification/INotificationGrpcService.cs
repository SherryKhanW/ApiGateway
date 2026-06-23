using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace NotificationService.Grpc.Contracts;

[Service]
public interface INotificationGrpcService
{
    Task<SendOtpEmailResponse> SendOtpEmailAsync(
        SendOtpEmailRequest request,
        CallContext context = default);
}
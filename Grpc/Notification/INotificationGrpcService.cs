using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Configuration;

namespace ApiGateway.Grpc.Notification;

[Service]
public interface INotificationGrpcService
{
    Task<SendOtpEmailResponse> SendOtpEmailAsync(
        SendOtpEmailRequest request,
        CallContext context = default);
}
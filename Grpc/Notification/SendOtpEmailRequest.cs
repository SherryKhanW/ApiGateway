using System.Runtime.Serialization;

namespace ApiGateway.Grpc.Notification;

[DataContract]
public class SendOtpEmailRequest
{
    [DataMember(Order = 1)]
    public string Email { get; set; } = string.Empty;

    [DataMember(Order = 2)]
    public string Otp { get; set; } = string.Empty;
}
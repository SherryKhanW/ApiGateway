using System.Runtime.Serialization;

namespace NotificationService.Grpc.Contracts;

[DataContract]
public class SendOtpEmailRequest
{
    [DataMember(Order = 1)]
    public string Email { get; set; } = string.Empty;

    [DataMember(Order = 2)]
    public string Otp { get; set; } = string.Empty;
}
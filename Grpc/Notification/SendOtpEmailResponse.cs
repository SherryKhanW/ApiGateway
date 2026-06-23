using System.Runtime.Serialization;

namespace NotificationService.Grpc.Contracts;

[DataContract]
public class SendOtpEmailResponse
{
    [DataMember(Order = 1)]
    public bool Success { get; set; }

    [DataMember(Order = 2)]
    public string ErrorMessage { get; set; } = string.Empty;
}
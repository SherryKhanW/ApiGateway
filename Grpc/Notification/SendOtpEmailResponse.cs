using System.Runtime.Serialization;

namespace ApiGateway.Grpc.Notification;

[DataContract]
public class SendOtpEmailResponse
{
    [DataMember(Order = 1)]
    public bool Success { get; set; }

    [DataMember(Order = 2)]
    public string ErrorMessage { get; set; } = string.Empty;
}
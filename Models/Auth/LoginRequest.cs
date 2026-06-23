namespace ApiGateway.Models.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string DeviceName { get; set; } = string.Empty;

    public string DeviceType { get; set; } = string.Empty;

    public string DeviceToken { get; set; } = string.Empty;
    
    public string Otp { get; set; } = string.Empty;
}

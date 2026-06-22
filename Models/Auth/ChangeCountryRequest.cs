namespace ApiGateway.Models.Auth;

public class ChangeCountryRequest
{
    public string Country { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
}
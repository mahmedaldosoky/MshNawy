namespace MshNawy.Application.Identity;

public class JwtOptions
{
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public string SigningKey { get; set; } = "";
    public int ExpiresInMinutes { get; set; } = 60;
}

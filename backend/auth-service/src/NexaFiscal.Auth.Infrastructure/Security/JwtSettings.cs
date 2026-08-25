namespace NexaFiscal.Auth.Infrastructure.Security;

public sealed class JwtSettings
{
    public string Issuer { get; init; } = "NexaFiscal.Auth";
    public string Audience { get; init; } = "NexaFiscal";
    public string Key { get; init; } = string.Empty;
    public int ExpirationMinutes { get; init; } = 60;
}

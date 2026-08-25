namespace NexaFiscal.Auth.Domain.Entities;

public sealed class User
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
    public required string PasswordSalt { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

namespace NexaFiscal.Auth.Application.Models;

public sealed record AuthResult(
    string Token,
    DateTime ExpiresAt,
    AuthenticatedUser User);

public sealed record AuthenticatedUser(
    string Id,
    string Name,
    string Email);

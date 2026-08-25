using NexaFiscal.Auth.Application.Models;

namespace NexaFiscal.Auth.Api.Contracts;

public sealed record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    AuthenticatedUser User)
{
    public static AuthResponse From(AuthResult result) =>
        new(result.Token, result.ExpiresAt, result.User);
}

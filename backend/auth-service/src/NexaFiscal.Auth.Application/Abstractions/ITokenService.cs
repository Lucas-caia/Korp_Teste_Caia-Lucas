using NexaFiscal.Auth.Domain.Entities;

namespace NexaFiscal.Auth.Application.Abstractions;

public interface ITokenService
{
    TokenResult Create(User user);
}

public sealed record TokenResult(string Token, DateTime ExpiresAt);

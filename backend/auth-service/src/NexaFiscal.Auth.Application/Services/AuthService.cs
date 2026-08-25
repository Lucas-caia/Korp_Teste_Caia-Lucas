using NexaFiscal.Auth.Application.Abstractions;
using NexaFiscal.Auth.Application.Models;
using NexaFiscal.Auth.Domain.Entities;
using NexaFiscal.Auth.Domain.Exceptions;

namespace NexaFiscal.Auth.Application.Services;

public sealed class AuthService(
    IUserRepository users,
    IPasswordService passwords,
    ITokenService tokens)
{
    public async Task<AuthResult> RegisterAsync(
        string name,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var existing = await users.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (existing is not null)
        {
            throw new EmailAlreadyRegisteredException();
        }

        var passwordHash = passwords.Hash(password);
        var user = new User
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            PasswordHash = passwordHash.Hash,
            PasswordSalt = passwordHash.Salt
        };

        await users.AddAsync(user, cancellationToken);
        return BuildResult(user);
    }

    public async Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var user = await users.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null || !passwords.Verify(password, user.PasswordHash, user.PasswordSalt))
        {
            throw new InvalidCredentialsException();
        }

        return BuildResult(user);
    }

    private AuthResult BuildResult(User user)
    {
        var token = tokens.Create(user);
        return new AuthResult(
            token.Token,
            token.ExpiresAt,
            new AuthenticatedUser(user.Id, user.Name, user.Email));
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}

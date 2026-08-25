using System.Security.Cryptography;
using NexaFiscal.Auth.Application.Abstractions;

namespace NexaFiscal.Auth.Infrastructure.Security;

public sealed class PasswordService : IPasswordService
{
    private const int Iterations = 210_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    public PasswordHashResult Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);

        return new PasswordHashResult(
            Convert.ToBase64String(hash),
            Convert.ToBase64String(salt));
    }

    public bool Verify(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var expectedHash = Convert.FromBase64String(hash);
        var actualHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA256,
            expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using NexaFiscal.Auth.Application.Abstractions;
using NexaFiscal.Auth.Domain.Entities;
using NexaFiscal.Auth.Domain.Exceptions;
using NexaFiscal.Auth.Infrastructure.Persistence;

namespace NexaFiscal.Auth.Infrastructure.Repositories;

public sealed class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserDocument> _users;

    public MongoUserRepository(MongoSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        _users = database.GetCollection<UserDocument>("users");
    }

    public async Task<User?> GetByEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default)
    {
        var document = await _users
            .Find(user => user.Email == normalizedEmail)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            await _users.InsertOneAsync(
                UserDocument.FromDomain(user),
                cancellationToken: cancellationToken);
        }
        catch (MongoWriteException exception)
            when (exception.WriteError?.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new EmailAlreadyRegisteredException();
        }
    }

    public async Task EnsureIndexesAsync(CancellationToken cancellationToken = default)
    {
        var keys = Builders<UserDocument>.IndexKeys.Ascending(user => user.Email);
        var options = new CreateIndexOptions { Unique = true, Name = "ux_users_email" };
        await _users.Indexes.CreateOneAsync(
            new CreateIndexModel<UserDocument>(keys, options),
            cancellationToken: cancellationToken);
    }

    private sealed class UserDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public required string Id { get; init; }

        public required string Name { get; init; }
        public required string Email { get; init; }
        public required string PasswordHash { get; init; }
        public required string PasswordSalt { get; init; }
        public DateTime CreatedAt { get; init; }

        public User ToDomain() => new()
        {
            Id = Id,
            Name = Name,
            Email = Email,
            PasswordHash = PasswordHash,
            PasswordSalt = PasswordSalt,
            CreatedAt = CreatedAt
        };

        public static UserDocument FromDomain(User user) => new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            PasswordSalt = user.PasswordSalt,
            CreatedAt = user.CreatedAt
        };
    }
}

namespace NexaFiscal.Auth.Infrastructure.Persistence;

public sealed class MongoSettings
{
    public string ConnectionString { get; init; } = "mongodb://localhost:27017";
    public string DatabaseName { get; init; } = "NexaFiscalAuth";
}

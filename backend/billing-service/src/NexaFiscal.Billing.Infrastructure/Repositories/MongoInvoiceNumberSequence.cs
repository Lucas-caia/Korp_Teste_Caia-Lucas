using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexaFiscal.Billing.Application.Abstractions;
using NexaFiscal.Billing.Infrastructure.Persistence;

namespace NexaFiscal.Billing.Infrastructure.Repositories;

public sealed class MongoInvoiceNumberSequence : IInvoiceNumberSequence
{
    private readonly IMongoCollection<InvoiceCounterDocument> _counters;

    public MongoInvoiceNumberSequence(IOptions<MongoSettings> settings)
    {
        var configuration = settings.Value;
        var client = new MongoClient(configuration.ConnectionString);
        var database = client.GetDatabase(configuration.DatabaseName);
        _counters = database.GetCollection<InvoiceCounterDocument>("counters");
    }

    public async Task<string> NextAsync(CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var counterId = $"invoice-{year}";

        var counter = await _counters.FindOneAndUpdateAsync(
            document => document.Id == counterId,
            Builders<InvoiceCounterDocument>.Update
                .SetOnInsert(document => document.Id, counterId)
                .Inc(document => document.Sequence, 1),
            new FindOneAndUpdateOptions<InvoiceCounterDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            },
            cancellationToken);

        return $"NF-{year}-{counter.Sequence:D3}";
    }
}

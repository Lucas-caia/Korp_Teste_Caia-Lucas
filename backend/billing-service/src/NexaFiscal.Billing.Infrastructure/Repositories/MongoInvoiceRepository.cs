using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexaFiscal.Billing.Application.Abstractions;
using NexaFiscal.Billing.Domain.Entities;
using NexaFiscal.Billing.Infrastructure.Persistence;

namespace NexaFiscal.Billing.Infrastructure.Repositories;

public sealed class MongoInvoiceRepository : IInvoiceRepository
{
    private readonly IMongoCollection<InvoiceDocument> _invoices;

    public MongoInvoiceRepository(IOptions<MongoSettings> settings)
    {
        var configuration = settings.Value;
        var client = new MongoClient(configuration.ConnectionString);
        var database = client.GetDatabase(configuration.DatabaseName);
        _invoices = database.GetCollection<InvoiceDocument>("invoices");

        var uniqueNumber = new CreateIndexModel<InvoiceDocument>(
            Builders<InvoiceDocument>.IndexKeys.Ascending(invoice => invoice.Number),
            new CreateIndexOptions { Unique = true, Name = "ux_invoices_number" });

        _invoices.Indexes.CreateOne(uniqueNumber);
    }

    public async Task<IReadOnlyCollection<Invoice>> ListAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _invoices
            .Find(FilterDefinition<InvoiceDocument>.Empty)
            .SortByDescending(invoice => invoice.CreatedAt)
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public async Task<Invoice?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var document = await _invoices
            .Find(invoice => invoice.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default) =>
        _invoices.InsertOneAsync(
            InvoiceDocument.FromDomain(invoice),
            cancellationToken: cancellationToken);

    public async Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var result = await _invoices.ReplaceOneAsync(
            current => current.Id == invoice.Id,
            InvoiceDocument.FromDomain(invoice),
            cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
            throw new InvalidOperationException($"Nota fiscal '{invoice.Id}' não encontrada para atualização.");
    }
}

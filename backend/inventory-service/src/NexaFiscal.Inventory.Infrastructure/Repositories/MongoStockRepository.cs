using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexaFiscal.Inventory.Application.Abstractions;
using NexaFiscal.Inventory.Application.Models;
using NexaFiscal.Inventory.Infrastructure.Persistence;

namespace NexaFiscal.Inventory.Infrastructure.Repositories;

public sealed class MongoStockRepository : IStockRepository
{
    private readonly IMongoCollection<ProductDocument> _products;

    public MongoStockRepository(IOptions<MongoSettings> settings)
    {
        var configuration = settings.Value;
        var client = new MongoClient(configuration.ConnectionString);
        var database = client.GetDatabase(configuration.DatabaseName);
        _products = database.GetCollection<ProductDocument>("products");
    }

    public async Task<IReadOnlyCollection<StockShortage>> ConsumeAsync(
        IReadOnlyCollection<StockConsumptionItem> items,
        CancellationToken cancellationToken = default)
    {
        var shortages = await FindShortagesAsync(items, cancellationToken);
        if (shortages.Count > 0)
            return shortages;

        var consumed = new List<StockConsumptionItem>();

        try
        {
            foreach (var item in items)
            {
                var filter = Builders<ProductDocument>.Filter.And(
                    Builders<ProductDocument>.Filter.Eq(product => product.Id, item.ProductId),
                    Builders<ProductDocument>.Filter.Gte(product => product.Balance, item.Quantity));

                var update = Builders<ProductDocument>.Update.Inc(product => product.Balance, -item.Quantity);

                var result = await _products.UpdateOneAsync(
                    filter,
                    update,
                    cancellationToken: cancellationToken);

                if (result.ModifiedCount == 0)
                {
                    await RollbackAsync(consumed, cancellationToken);

                    var currentShortages = await FindShortagesAsync(items, cancellationToken);
                    return currentShortages.Count > 0
                        ? currentShortages
                        : new[]
                        {
                            new StockShortage(
                                item.ProductId,
                                "—",
                                "Produto indisponível",
                                item.Quantity,
                                0)
                        };
                }

                consumed.Add(item);
            }

            return Array.Empty<StockShortage>();
        }
        catch
        {
            await RollbackAsync(consumed, CancellationToken.None);
            throw;
        }
    }

    private async Task<IReadOnlyCollection<StockShortage>> FindShortagesAsync(
        IReadOnlyCollection<StockConsumptionItem> items,
        CancellationToken cancellationToken)
    {
        var ids = items.Select(item => item.ProductId).Distinct().ToArray();

        var documents = await _products
            .Find(product => ids.Contains(product.Id))
            .ToListAsync(cancellationToken);

        var productsById = documents.ToDictionary(product => product.Id, StringComparer.Ordinal);
        var shortages = new List<StockShortage>();

        foreach (var item in items)
        {
            if (!productsById.TryGetValue(item.ProductId, out var product))
            {
                shortages.Add(new StockShortage(
                    item.ProductId,
                    "—",
                    "Produto não encontrado",
                    item.Quantity,
                    0));
                continue;
            }

            if (product.Balance < item.Quantity)
            {
                shortages.Add(new StockShortage(
                    product.Id,
                    product.Code,
                    product.Description,
                    item.Quantity,
                    product.Balance));
            }
        }

        return shortages;
    }

    private async Task RollbackAsync(
        IReadOnlyCollection<StockConsumptionItem> consumed,
        CancellationToken cancellationToken)
    {
        foreach (var item in consumed)
        {
            var update = Builders<ProductDocument>.Update.Inc(product => product.Balance, item.Quantity);

            await _products.UpdateOneAsync(
                product => product.Id == item.ProductId,
                update,
                cancellationToken: cancellationToken);
        }
    }
}

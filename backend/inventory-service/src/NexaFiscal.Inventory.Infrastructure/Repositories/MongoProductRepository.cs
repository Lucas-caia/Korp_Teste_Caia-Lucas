using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NexaFiscal.Inventory.Application.Abstractions;
using NexaFiscal.Inventory.Application.Exceptions;
using NexaFiscal.Inventory.Domain.Entities;
using NexaFiscal.Inventory.Infrastructure.Persistence;

namespace NexaFiscal.Inventory.Infrastructure.Repositories;

public sealed class MongoProductRepository : IProductRepository
{
    private readonly IMongoCollection<ProductDocument> _products;

    public MongoProductRepository(IOptions<MongoSettings> settings)
    {
        var configuration = settings.Value;
        var client = new MongoClient(configuration.ConnectionString);
        var database = client.GetDatabase(configuration.DatabaseName);
        _products = database.GetCollection<ProductDocument>("products");

        var uniqueCode = new CreateIndexModel<ProductDocument>(
            Builders<ProductDocument>.IndexKeys.Ascending(product => product.Code),
            new CreateIndexOptions { Unique = true, Name = "ux_products_code" });

        _products.Indexes.CreateOne(uniqueCode);
    }

    public async Task<IReadOnlyCollection<Product>> ListAsync(CancellationToken cancellationToken = default)
    {
        var documents = await _products
            .Find(FilterDefinition<ProductDocument>.Empty)
            .SortBy(product => product.Description)
            .ToListAsync(cancellationToken);

        return documents.Select(document => document.ToDomain()).ToArray();
    }

    public async Task<Product?> FindByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var document = await _products
            .Find(product => product.Code == code)
            .FirstOrDefaultAsync(cancellationToken);

        return document?.ToDomain();
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        try
        {
            await _products.InsertOneAsync(ProductDocument.FromDomain(product), cancellationToken: cancellationToken);
        }
        catch (MongoWriteException exception) when (exception.WriteError?.Code == 11000)
        {
            throw new ProductCodeAlreadyExistsException(product.Code);
        }
    }
}

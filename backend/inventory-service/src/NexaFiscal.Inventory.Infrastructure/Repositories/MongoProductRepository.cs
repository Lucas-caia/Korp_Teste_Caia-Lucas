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
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private volatile bool _initialized;

    public MongoProductRepository(IOptions<MongoSettings> settings)
    {
        var configuration = settings.Value;

        if (string.IsNullOrWhiteSpace(configuration.ConnectionString))
            throw new InvalidOperationException("Mongo:ConnectionString não configurada para o Inventory Service.");

        if (string.IsNullOrWhiteSpace(configuration.DatabaseName))
            throw new InvalidOperationException("Mongo:DatabaseName não configurado para o Inventory Service.");

        var client = new MongoClient(configuration.ConnectionString);
        var database = client.GetDatabase(configuration.DatabaseName);

        _products = database.GetCollection<ProductDocument>("products");
    }

    public async Task<IReadOnlyCollection<Product>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureInitializedAsync(cancellationToken);

            var documents = await _products
                .Find(FilterDefinition<ProductDocument>.Empty)
                .SortBy(product => product.Description)
                .ToListAsync(cancellationToken);

            return documents.Select(document => document.ToDomain()).ToArray();
        }
        catch (MongoException exception)
        {
            throw PersistenceFailure(exception);
        }
    }

    public async Task<Product?> FindByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureInitializedAsync(cancellationToken);

            var document = await _products
                .Find(product => product.Code == code)
                .FirstOrDefaultAsync(cancellationToken);

            return document?.ToDomain();
        }
        catch (MongoException exception)
        {
            throw PersistenceFailure(exception);
        }
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureInitializedAsync(cancellationToken);

            await _products.InsertOneAsync(
                ProductDocument.FromDomain(product),
                cancellationToken: cancellationToken);
        }
        catch (MongoWriteException exception) when (exception.WriteError?.Code == 11000)
        {
            throw new ProductCodeAlreadyExistsException(product.Code);
        }
        catch (MongoException exception)
        {
            throw PersistenceFailure(exception);
        }
    }

    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_initialized)
            return;

        await _initializationLock.WaitAsync(cancellationToken);

        try
        {
            if (_initialized)
                return;

            var uniqueCode = new CreateIndexModel<ProductDocument>(
                Builders<ProductDocument>.IndexKeys.Ascending(product => product.Code),
                new CreateIndexOptions
                {
                    Unique = true,
                    Name = "ux_products_code"
                });

            await _products.Indexes.CreateOneAsync(
                uniqueCode,
                cancellationToken: cancellationToken);

            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private static InventoryPersistenceException PersistenceFailure(MongoException exception) =>
        new(
            "Não foi possível acessar o banco de dados do serviço de estoque.",
            exception);
}

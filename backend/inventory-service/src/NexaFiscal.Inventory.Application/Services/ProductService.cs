using NexaFiscal.Inventory.Application.Abstractions;
using NexaFiscal.Inventory.Application.Exceptions;
using NexaFiscal.Inventory.Domain.Entities;

namespace NexaFiscal.Inventory.Application.Services;

public sealed class ProductService(IProductRepository repository)
{
    public Task<IReadOnlyCollection<Product>> ListAsync(CancellationToken cancellationToken = default) =>
        repository.ListAsync(cancellationToken);

    public async Task<Product> CreateAsync(
        string code,
        string description,
        CancellationToken cancellationToken = default)
    {
        var product = Product.Create(code, description);
        var existing = await repository.FindByCodeAsync(product.Code, cancellationToken);

        if (existing is not null)
            throw new ProductCodeAlreadyExistsException(product.Code);

        await repository.AddAsync(product, cancellationToken);
        return product;
    }
}

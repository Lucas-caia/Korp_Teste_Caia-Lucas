using NexaFiscal.Inventory.Domain.Entities;

namespace NexaFiscal.Inventory.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyCollection<Product>> ListAsync(CancellationToken cancellationToken = default);
    Task<Product?> FindByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
}

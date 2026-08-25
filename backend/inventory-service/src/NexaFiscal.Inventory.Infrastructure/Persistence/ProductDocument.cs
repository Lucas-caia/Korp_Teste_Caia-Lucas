using NexaFiscal.Inventory.Domain.Entities;

namespace NexaFiscal.Inventory.Infrastructure.Persistence;

internal sealed class ProductDocument
{
    public string Id { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int Balance { get; init; }
    public DateTime CreatedAt { get; init; }

    public static ProductDocument FromDomain(Product product) => new()
    {
        Id = product.Id,
        Code = product.Code,
        Description = product.Description,
        Balance = product.Balance,
        CreatedAt = product.CreatedAt
    };

    public Product ToDomain() =>
        Product.Restore(Id, Code, Description, Balance, CreatedAt);
}

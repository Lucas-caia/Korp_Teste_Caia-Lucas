using NexaFiscal.Inventory.Domain.Entities;

namespace NexaFiscal.Inventory.Api.Contracts;

public sealed record ProductResponse(
    string Id,
    string Code,
    string Description,
    DateTime CreatedAt)
{
    public static ProductResponse FromDomain(Product product) =>
        new(product.Id, product.Code, product.Description, product.CreatedAt);
}

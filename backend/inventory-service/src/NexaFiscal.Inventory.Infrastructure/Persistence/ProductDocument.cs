using MongoDB.Bson.Serialization.Attributes;
using NexaFiscal.Inventory.Domain.Entities;

namespace NexaFiscal.Inventory.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
internal sealed class ProductDocument
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("code")]
    public string Code { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("balance")]
    public int Balance { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

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

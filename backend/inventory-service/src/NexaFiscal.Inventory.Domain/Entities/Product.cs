namespace NexaFiscal.Inventory.Domain.Entities;

public sealed class Product
{
    public string Id { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product(string id, string code, string description, DateTime createdAt)
    {
        Id = id;
        Code = code;
        Description = description;
        CreatedAt = createdAt;
    }

    public static Product Create(string code, string description)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var normalizedDescription = description.Trim();

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new ArgumentException("O código do produto é obrigatório.", nameof(code));

        if (string.IsNullOrWhiteSpace(normalizedDescription))
            throw new ArgumentException("A descrição do produto é obrigatória.", nameof(description));

        return new Product(
            Guid.NewGuid().ToString("N"),
            normalizedCode,
            normalizedDescription,
            DateTime.UtcNow);
    }

    public static Product Restore(string id, string code, string description, DateTime createdAt) =>
        new(id, code, description, createdAt);
}

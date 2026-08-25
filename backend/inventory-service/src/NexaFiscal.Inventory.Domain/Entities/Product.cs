namespace NexaFiscal.Inventory.Domain.Entities;

public sealed class Product
{
    public string Id { get; private set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public int Balance { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Product(
        string id,
        string code,
        string description,
        int balance,
        DateTime createdAt)
    {
        Id = id;
        Code = code;
        Description = description;
        Balance = balance;
        CreatedAt = createdAt;
    }

    public static Product Create(string code, string description, int balance)
    {
        var normalizedCode = code.Trim().ToUpperInvariant();
        var normalizedDescription = description.Trim();

        if (string.IsNullOrWhiteSpace(normalizedCode))
            throw new ArgumentException("O código do produto é obrigatório.", nameof(code));

        if (string.IsNullOrWhiteSpace(normalizedDescription))
            throw new ArgumentException("A descrição do produto é obrigatória.", nameof(description));

        if (balance < 0)
            throw new ArgumentOutOfRangeException(nameof(balance), "O saldo não pode ser negativo.");

        return new Product(
            Guid.NewGuid().ToString("N"),
            normalizedCode,
            normalizedDescription,
            balance,
            DateTime.UtcNow);
    }

    public static Product Restore(
        string id,
        string code,
        string description,
        int balance,
        DateTime createdAt) =>
        new(id, code, description, balance, createdAt);
}

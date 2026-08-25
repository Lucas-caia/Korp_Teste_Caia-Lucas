namespace NexaFiscal.Billing.Domain.Entities;

public sealed class InvoiceItem
{
    public string ProductId { get; private set; }
    public int Quantity { get; private set; }

    private InvoiceItem(string productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public static InvoiceItem Create(string productId, int quantity)
    {
        var normalizedProductId = productId.Trim();

        if (string.IsNullOrWhiteSpace(normalizedProductId))
            throw new ArgumentException("O produto é obrigatório.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "A quantidade deve ser maior que zero.");

        return new InvoiceItem(normalizedProductId, quantity);
    }

    public static InvoiceItem Restore(string productId, int quantity) =>
        new(productId, quantity);
}

using NexaFiscal.Billing.Domain.Entities;

namespace NexaFiscal.Billing.Infrastructure.Persistence;

internal sealed class InvoiceItemDocument
{
    public string ProductId { get; init; } = string.Empty;
    public int Quantity { get; init; }

    public static InvoiceItemDocument FromDomain(InvoiceItem item) => new()
    {
        ProductId = item.ProductId,
        Quantity = item.Quantity
    };

    public InvoiceItem ToDomain() =>
        InvoiceItem.Restore(ProductId, Quantity);
}

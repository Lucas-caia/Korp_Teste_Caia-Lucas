using NexaFiscal.Billing.Domain.Entities;

namespace NexaFiscal.Billing.Infrastructure.Persistence;

internal sealed class InvoiceDocument
{
    public string Id { get; init; } = string.Empty;
    public string Number { get; init; } = string.Empty;
    public InvoiceStatus Status { get; init; }
    public List<InvoiceItemDocument> Items { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public DateTime? ClosedAt { get; init; }

    public static InvoiceDocument FromDomain(Invoice invoice) => new()
    {
        Id = invoice.Id,
        Number = invoice.Number,
        Status = invoice.Status,
        Items = invoice.Items.Select(InvoiceItemDocument.FromDomain).ToList(),
        CreatedAt = invoice.CreatedAt,
        ClosedAt = invoice.ClosedAt
    };

    public Invoice ToDomain() => Invoice.Restore(
        Id,
        Number,
        Status,
        Items.Select(item => item.ToDomain()),
        CreatedAt,
        ClosedAt);
}

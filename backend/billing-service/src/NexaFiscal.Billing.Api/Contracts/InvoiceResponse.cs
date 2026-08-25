using NexaFiscal.Billing.Domain.Entities;

namespace NexaFiscal.Billing.Api.Contracts;

public sealed record InvoiceItemResponse(
    string ProductId,
    int Quantity)
{
    public static InvoiceItemResponse FromDomain(InvoiceItem item) =>
        new(item.ProductId, item.Quantity);
}

public sealed record InvoiceResponse(
    string Id,
    string Number,
    string Status,
    IReadOnlyCollection<InvoiceItemResponse> Items,
    DateTime CreatedAt,
    DateTime? ClosedAt)
{
    public static InvoiceResponse FromDomain(Invoice invoice) => new(
        invoice.Id,
        invoice.Number,
        invoice.Status == InvoiceStatus.Open ? "OPEN" : "CLOSED",
        invoice.Items.Select(InvoiceItemResponse.FromDomain).ToArray(),
        invoice.CreatedAt,
        invoice.ClosedAt);
}

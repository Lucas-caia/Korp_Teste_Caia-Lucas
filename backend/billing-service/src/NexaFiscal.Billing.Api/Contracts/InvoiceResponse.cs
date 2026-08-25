using NexaFiscal.Billing.Domain.Entities;

namespace NexaFiscal.Billing.Api.Contracts;

public sealed record InvoiceResponse(
    string Id,
    string Number,
    string Status,
    DateTime CreatedAt,
    DateTime? ClosedAt)
{
    public static InvoiceResponse FromDomain(Invoice invoice) => new(
        invoice.Id,
        invoice.Number,
        invoice.Status == InvoiceStatus.Open ? "OPEN" : "CLOSED",
        invoice.CreatedAt,
        invoice.ClosedAt);
}

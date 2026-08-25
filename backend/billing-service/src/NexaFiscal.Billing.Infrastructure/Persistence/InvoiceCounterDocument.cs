namespace NexaFiscal.Billing.Infrastructure.Persistence;

internal sealed class InvoiceCounterDocument
{
    public string Id { get; init; } = string.Empty;
    public long Sequence { get; init; }
}

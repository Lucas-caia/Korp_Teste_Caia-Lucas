namespace NexaFiscal.Billing.Domain.Entities;

public sealed class Invoice
{
    public string Id { get; private set; }
    public string Number { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    private Invoice(
        string id,
        string number,
        InvoiceStatus status,
        DateTime createdAt,
        DateTime? closedAt)
    {
        Id = id;
        Number = number;
        Status = status;
        CreatedAt = createdAt;
        ClosedAt = closedAt;
    }

    public static Invoice Open(string number) => new(
        Guid.NewGuid().ToString("N"),
        number,
        InvoiceStatus.Open,
        DateTime.UtcNow,
        null);

    public static Invoice Restore(
        string id,
        string number,
        InvoiceStatus status,
        DateTime createdAt,
        DateTime? closedAt) =>
        new(id, number, status, createdAt, closedAt);
}

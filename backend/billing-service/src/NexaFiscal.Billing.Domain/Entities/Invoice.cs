namespace NexaFiscal.Billing.Domain.Entities;

public sealed class Invoice
{
    private readonly List<InvoiceItem> _items;

    public string Id { get; private set; }
    public string Number { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    private Invoice(
        string id,
        string number,
        InvoiceStatus status,
        IEnumerable<InvoiceItem> items,
        DateTime createdAt,
        DateTime? closedAt)
    {
        Id = id;
        Number = number;
        Status = status;
        _items = items.ToList();
        CreatedAt = createdAt;
        ClosedAt = closedAt;
    }

    public static Invoice Open(string number, IEnumerable<InvoiceItem> items)
    {
        var itemList = items.ToList();

        if (itemList.Count == 0)
            throw new ArgumentException("A nota fiscal deve possuir ao menos um produto.", nameof(items));

        var hasDuplicateProducts = itemList
            .GroupBy(item => item.ProductId, StringComparer.Ordinal)
            .Any(group => group.Count() > 1);

        if (hasDuplicateProducts)
            throw new ArgumentException("Um mesmo produto não pode ser incluído mais de uma vez na nota.", nameof(items));

        return new Invoice(
            Guid.NewGuid().ToString("N"),
            number,
            InvoiceStatus.Open,
            itemList,
            DateTime.UtcNow,
            null);
    }

    public static Invoice Restore(
        string id,
        string number,
        InvoiceStatus status,
        IEnumerable<InvoiceItem> items,
        DateTime createdAt,
        DateTime? closedAt) =>
        new(id, number, status, items, createdAt, closedAt);
}

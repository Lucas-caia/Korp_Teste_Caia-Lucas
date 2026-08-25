using System.ComponentModel.DataAnnotations;

namespace NexaFiscal.Billing.Api.Contracts;

public sealed class CreateInvoiceRequest
{
    [Required]
    [MinLength(1)]
    public IReadOnlyCollection<CreateInvoiceItemRequest> Items { get; init; } =
        Array.Empty<CreateInvoiceItemRequest>();
}

public sealed class CreateInvoiceItemRequest
{
    [Required]
    public string ProductId { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}

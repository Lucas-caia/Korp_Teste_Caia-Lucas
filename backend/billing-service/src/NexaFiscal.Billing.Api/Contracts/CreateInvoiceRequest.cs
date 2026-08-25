using System.ComponentModel.DataAnnotations;

namespace NexaFiscal.Billing.Api.Contracts;

public sealed record CreateInvoiceRequest(
    [property: Required, MinLength(1)] IReadOnlyCollection<CreateInvoiceItemRequest> Items);

public sealed record CreateInvoiceItemRequest(
    [property: Required] string ProductId,
    [property: Range(1, int.MaxValue)] int Quantity);

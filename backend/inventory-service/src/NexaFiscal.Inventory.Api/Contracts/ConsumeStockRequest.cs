using System.ComponentModel.DataAnnotations;

namespace NexaFiscal.Inventory.Api.Contracts;

public sealed record ConsumeStockRequest(
    [property: Required, MinLength(1)] IReadOnlyCollection<ConsumeStockItemRequest> Items);

public sealed record ConsumeStockItemRequest(
    [property: Required] string ProductId,
    [property: Range(1, int.MaxValue)] int Quantity);

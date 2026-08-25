using System.ComponentModel.DataAnnotations;

namespace NexaFiscal.Inventory.Api.Contracts;

public sealed class ConsumeStockRequest
{
    [Required]
    [MinLength(1)]
    public IReadOnlyCollection<ConsumeStockItemRequest> Items { get; init; } =
        Array.Empty<ConsumeStockItemRequest>();
}

public sealed class ConsumeStockItemRequest
{
    [Required]
    public string ProductId { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}

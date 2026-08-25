using System.ComponentModel.DataAnnotations;

namespace NexaFiscal.Inventory.Api.Contracts;

public sealed class CreateProductRequest
{
    [Required]
    [MaxLength(40)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [MaxLength(160)]
    public string Description { get; init; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Balance { get; init; }
}

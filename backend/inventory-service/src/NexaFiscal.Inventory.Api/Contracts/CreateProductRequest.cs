using System.ComponentModel.DataAnnotations;

namespace NexaFiscal.Inventory.Api.Contracts;

public sealed record CreateProductRequest(
    [property: Required, MaxLength(40)] string Code,
    [property: Required, MaxLength(160)] string Description);

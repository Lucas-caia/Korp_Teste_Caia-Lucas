using NexaFiscal.Inventory.Application.Models;

namespace NexaFiscal.Inventory.Application.Exceptions;

public sealed class InsufficientStockException(IReadOnlyCollection<StockShortage> items)
    : Exception("Estoque insuficiente para concluir a nota fiscal.")
{
    public IReadOnlyCollection<StockShortage> Items { get; } = items;
}

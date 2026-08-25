using NexaFiscal.Inventory.Application.Models;

namespace NexaFiscal.Inventory.Application.Abstractions;

public interface IStockRepository
{
    Task<IReadOnlyCollection<StockShortage>> ConsumeAsync(
        IReadOnlyCollection<StockConsumptionItem> items,
        CancellationToken cancellationToken = default);
}

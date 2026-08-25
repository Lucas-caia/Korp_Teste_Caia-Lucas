using NexaFiscal.Billing.Application.Models;

namespace NexaFiscal.Billing.Application.Abstractions;

public interface IInventoryGateway
{
    Task ConsumeAsync(
        IReadOnlyCollection<StockConsumptionItem> items,
        CancellationToken cancellationToken = default);
}

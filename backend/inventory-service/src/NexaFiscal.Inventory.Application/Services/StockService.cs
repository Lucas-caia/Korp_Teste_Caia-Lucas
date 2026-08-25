using NexaFiscal.Inventory.Application.Abstractions;
using NexaFiscal.Inventory.Application.Exceptions;
using NexaFiscal.Inventory.Application.Models;

namespace NexaFiscal.Inventory.Application.Services;

public sealed class StockService(IStockRepository stockRepository)
{
    public async Task ConsumeAsync(
        IReadOnlyCollection<StockConsumptionItem> items,
        CancellationToken cancellationToken = default)
    {
        if (items.Count == 0)
            throw new ArgumentException("Ao menos um produto é necessário para realizar a baixa.", nameof(items));

        if (items.Any(item => string.IsNullOrWhiteSpace(item.ProductId) || item.Quantity <= 0))
            throw new ArgumentException("Os itens de estoque informados são inválidos.", nameof(items));

        var normalizedItems = items
            .GroupBy(item => item.ProductId.Trim(), StringComparer.Ordinal)
            .Select(group => new StockConsumptionItem(
                group.Key,
                checked(group.Sum(item => item.Quantity))))
            .ToArray();

        var shortages = await stockRepository.ConsumeAsync(normalizedItems, cancellationToken);

        if (shortages.Count > 0)
            throw new InsufficientStockException(shortages);
    }
}

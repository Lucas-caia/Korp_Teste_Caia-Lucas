namespace NexaFiscal.Inventory.Application.Models;

public sealed record StockConsumptionItem(string ProductId, int Quantity);

public sealed record StockShortage(
    string ProductId,
    string Code,
    string Description,
    int Requested,
    int Available);

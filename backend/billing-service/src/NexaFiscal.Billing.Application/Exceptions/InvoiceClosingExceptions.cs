using NexaFiscal.Billing.Application.Models;

namespace NexaFiscal.Billing.Application.Exceptions;

public sealed class InvoiceNotFoundException(string id)
    : Exception($"Nota fiscal '{id}' não encontrada.");

public sealed class InvoiceAlreadyClosedException(string number)
    : Exception($"A nota fiscal {number} já está fechada.");

public sealed class InventoryUnavailableException(string message, Exception? innerException = null)
    : Exception(message, innerException);

public sealed class StockInsufficientException(IReadOnlyCollection<StockShortage> items)
    : Exception("Estoque insuficiente para concluir a nota fiscal.")
{
    public IReadOnlyCollection<StockShortage> Items { get; } = items;
}

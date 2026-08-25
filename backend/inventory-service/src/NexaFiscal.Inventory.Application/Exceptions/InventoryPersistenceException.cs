namespace NexaFiscal.Inventory.Application.Exceptions;

public sealed class InventoryPersistenceException(string message, Exception innerException)
    : Exception(message, innerException);

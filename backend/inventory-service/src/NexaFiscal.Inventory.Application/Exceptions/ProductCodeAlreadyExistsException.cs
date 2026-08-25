namespace NexaFiscal.Inventory.Application.Exceptions;

public sealed class ProductCodeAlreadyExistsException(string code)
    : Exception($"Já existe um produto com o código {code}.")
{
    public string Code { get; } = code;
}

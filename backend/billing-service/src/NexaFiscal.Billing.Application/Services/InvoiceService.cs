using NexaFiscal.Billing.Application.Abstractions;
using NexaFiscal.Billing.Application.Exceptions;
using NexaFiscal.Billing.Application.Models;
using NexaFiscal.Billing.Domain.Entities;

namespace NexaFiscal.Billing.Application.Services;

public sealed class InvoiceService(
    IInvoiceRepository repository,
    IInvoiceNumberSequence numberSequence,
    IInventoryGateway inventory)
{
    public Task<IReadOnlyCollection<Invoice>> ListAsync(CancellationToken cancellationToken = default) =>
        repository.ListAsync(cancellationToken);

    public Task<Invoice?> FindByIdAsync(string id, CancellationToken cancellationToken = default) =>
        repository.FindByIdAsync(id, cancellationToken);

    public async Task<Invoice> CreateAsync(
        IReadOnlyCollection<CreateInvoiceItem> items,
        CancellationToken cancellationToken = default)
    {
        var domainItems = items
            .Select(item => InvoiceItem.Create(item.ProductId, item.Quantity))
            .ToArray();

        var number = await numberSequence.NextAsync(cancellationToken);
        var invoice = Invoice.Open(number, domainItems);

        await repository.AddAsync(invoice, cancellationToken);
        return invoice;
    }

    public async Task<Invoice> CloseAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        var invoice = await repository.FindByIdAsync(id, cancellationToken)
            ?? throw new InvoiceNotFoundException(id);

        if (invoice.Status != InvoiceStatus.Open)
            throw new InvoiceAlreadyClosedException(invoice.Number);

        var stockItems = invoice.Items
            .Select(item => new StockConsumptionItem(item.ProductId, item.Quantity))
            .ToArray();

        await inventory.ConsumeAsync(stockItems, cancellationToken);

        invoice.Close();
        await repository.UpdateAsync(invoice, cancellationToken);

        return invoice;
    }
}

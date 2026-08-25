namespace NexaFiscal.Billing.Application.Abstractions;

public interface IInvoiceNumberSequence
{
    Task<string> NextAsync(CancellationToken cancellationToken = default);
}

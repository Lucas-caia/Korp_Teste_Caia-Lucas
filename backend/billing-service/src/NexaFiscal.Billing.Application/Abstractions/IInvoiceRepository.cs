using NexaFiscal.Billing.Domain.Entities;

namespace NexaFiscal.Billing.Application.Abstractions;

public interface IInvoiceRepository
{
    Task<IReadOnlyCollection<Invoice>> ListAsync(CancellationToken cancellationToken = default);
    Task<Invoice?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
}

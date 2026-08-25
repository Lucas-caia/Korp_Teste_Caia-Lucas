using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaFiscal.Billing.Api.Contracts;
using NexaFiscal.Billing.Application.Services;

namespace NexaFiscal.Billing.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/invoices")]
public sealed class InvoicesController(InvoiceService invoices) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<InvoiceResponse>>> List(CancellationToken cancellationToken)
    {
        var result = await invoices.ListAsync(cancellationToken);
        return Ok(result.Select(InvoiceResponse.FromDomain));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InvoiceResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        var invoice = await invoices.FindByIdAsync(id, cancellationToken);
        return invoice is null ? NotFound() : Ok(InvoiceResponse.FromDomain(invoice));
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceResponse>> Create(CancellationToken cancellationToken)
    {
        var invoice = await invoices.CreateAsync(cancellationToken);
        var response = InvoiceResponse.FromDomain(invoice);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }
}

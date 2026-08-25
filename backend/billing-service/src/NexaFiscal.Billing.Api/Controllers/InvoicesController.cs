using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaFiscal.Billing.Api.Contracts;
using NexaFiscal.Billing.Application.Models;
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
    public async Task<ActionResult<InvoiceResponse>> Create(
        [FromBody] CreateInvoiceRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = request.Items
                .Select(item => new CreateInvoiceItem(item.ProductId, item.Quantity))
                .ToArray();

            var invoice = await invoices.CreateAsync(items, cancellationToken);
            var response = InvoiceResponse.FromDomain(invoice);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Dados da nota fiscal inválidos",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaFiscal.Billing.Api.Contracts;
using NexaFiscal.Billing.Application.Exceptions;
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

    [HttpPost("{id}/close")]
    public async Task<ActionResult<InvoiceResponse>> Close(
        string id,
        CancellationToken cancellationToken)
    {
        try
        {
            var invoice = await invoices.CloseAsync(id, cancellationToken);
            return Ok(InvoiceResponse.FromDomain(invoice));
        }
        catch (InvoiceNotFoundException exception)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Nota fiscal não encontrada",
                Detail = exception.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
        catch (InvoiceAlreadyClosedException exception)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Nota fiscal já fechada",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (StockInsufficientException exception)
        {
            return Conflict(new
            {
                title = "Estoque insuficiente",
                detail = exception.Message,
                status = StatusCodes.Status409Conflict,
                items = exception.Items
            });
        }
        catch (InventoryUnavailableException exception)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
            {
                Title = "Serviço de estoque indisponível",
                Detail = exception.Message,
                Status = StatusCodes.Status503ServiceUnavailable
            });
        }
    }
}

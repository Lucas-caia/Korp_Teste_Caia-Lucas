using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaFiscal.Inventory.Api.Contracts;
using NexaFiscal.Inventory.Application.Exceptions;
using NexaFiscal.Inventory.Application.Models;
using NexaFiscal.Inventory.Application.Services;

namespace NexaFiscal.Inventory.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/stock")]
public sealed class StockController(StockService stock) : ControllerBase
{
    [HttpPost("consume")]
    public async Task<IActionResult> Consume(
        [FromBody] ConsumeStockRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = request.Items
                .Select(item => new StockConsumptionItem(item.ProductId, item.Quantity))
                .ToArray();

            await stock.ConsumeAsync(items, cancellationToken);
            return NoContent();
        }
        catch (InsufficientStockException exception)
        {
            return Conflict(new
            {
                title = "Estoque insuficiente",
                detail = exception.Message,
                status = StatusCodes.Status409Conflict,
                items = exception.Items
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Dados de estoque inválidos",
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}

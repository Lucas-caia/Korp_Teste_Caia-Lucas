using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaFiscal.Inventory.Api.Contracts;
using NexaFiscal.Inventory.Application.Exceptions;
using NexaFiscal.Inventory.Application.Services;

namespace NexaFiscal.Inventory.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public sealed class ProductsController(
    ProductService products,
    ILogger<ProductsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> List(
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await products.ListAsync(cancellationToken);
            return Ok(result.Select(ProductResponse.FromDomain));
        }
        catch (InventoryPersistenceException exception)
        {
            logger.LogError(exception, "Falha ao consultar produtos no Inventory Service.");

            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
            {
                Title = "Serviço de estoque indisponível",
                Detail = "Não foi possível consultar os produtos no momento.",
                Status = StatusCodes.Status503ServiceUnavailable
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await products.CreateAsync(
                request.Code,
                request.Description,
                request.Balance,
                cancellationToken);

            var response = ProductResponse.FromDomain(product);
            return Created($"/api/products/{response.Id}", response);
        }
        catch (ProductCodeAlreadyExistsException exception)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Código de produto já utilizado",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (InventoryPersistenceException exception)
        {
            logger.LogError(
                exception,
                "Falha ao cadastrar produto {ProductCode} no Inventory Service.",
                request.Code);

            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
            {
                Title = "Serviço de estoque indisponível",
                Detail = "Não foi possível salvar o produto no momento. Tente novamente em instantes.",
                Status = StatusCodes.Status503ServiceUnavailable
            });
        }
    }
}

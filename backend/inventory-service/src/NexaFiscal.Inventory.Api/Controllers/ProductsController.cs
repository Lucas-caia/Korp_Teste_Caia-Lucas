using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexaFiscal.Inventory.Api.Contracts;
using NexaFiscal.Inventory.Application.Exceptions;
using NexaFiscal.Inventory.Application.Services;

namespace NexaFiscal.Inventory.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public sealed class ProductsController(ProductService products) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> List(CancellationToken cancellationToken)
    {
        var result = await products.ListAsync(cancellationToken);
        return Ok(result.Select(ProductResponse.FromDomain));
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await products.CreateAsync(request.Code, request.Description, cancellationToken);
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
    }
}

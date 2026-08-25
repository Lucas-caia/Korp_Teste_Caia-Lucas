using Microsoft.AspNetCore.Mvc;
using NexaFiscal.Auth.Api.Contracts;
using NexaFiscal.Auth.Application.Services;
using NexaFiscal.Auth.Domain.Exceptions;

namespace NexaFiscal.Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.RegisterAsync(
                request.Name,
                request.Email,
                request.Password,
                cancellationToken);

            return StatusCode(StatusCodes.Status201Created, AuthResponse.From(result));
        }
        catch (EmailAlreadyRegisteredException exception)
        {
            return Conflict(new ProblemDetails
            {
                Title = "E-mail já cadastrado",
                Detail = exception.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authService.LoginAsync(
                request.Email,
                request.Password,
                cancellationToken);

            return Ok(AuthResponse.From(result));
        }
        catch (InvalidCredentialsException exception)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Credenciais inválidas",
                Detail = exception.Message,
                Status = StatusCodes.Status401Unauthorized
            });
        }
    }
}

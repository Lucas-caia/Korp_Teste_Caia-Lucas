using System.ComponentModel.DataAnnotations;

namespace NexaFiscal.Auth.Api.Contracts;

public sealed record RegisterRequest(
    [Required, MinLength(2), MaxLength(100)] string Name,
    [Required, EmailAddress, MaxLength(200)] string Email,
    [Required, MinLength(8), MaxLength(128)] string Password);

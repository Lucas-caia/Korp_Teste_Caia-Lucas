namespace NexaFiscal.Auth.Domain.Exceptions;

public sealed class EmailAlreadyRegisteredException : Exception
{
    public EmailAlreadyRegisteredException()
        : base("Já existe um usuário cadastrado com este e-mail.")
    {
    }
}

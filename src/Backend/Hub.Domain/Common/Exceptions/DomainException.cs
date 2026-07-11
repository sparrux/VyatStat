namespace Hub.Domain.Common.Exceptions;

sealed class DomainException : Exception
{
    internal DomainException(string? message) : base(message)
    {
    }
}
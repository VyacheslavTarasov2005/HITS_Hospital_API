namespace HITS_API_1.Application.Exceptions;

public class ForbiddenOperationException : Exception
{
    public ForbiddenOperationException(String message) : base(message) { }
}
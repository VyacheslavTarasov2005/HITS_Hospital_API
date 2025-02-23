namespace HITS_API_1.Domain.Exceptions;

public class IncorrectFieldException : FieldException
{
    public IncorrectFieldException(String fieldName, String message) : base(fieldName, message) { }
}
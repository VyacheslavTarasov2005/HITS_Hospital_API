using HITS_API_1.Domain.Exceptions;

namespace HITS_API_1.Application.Exceptions;

public class NotFoundFieldException : FieldException
{
    public NotFoundFieldException(String fieldName, String message) : base(fieldName, message) { }
}
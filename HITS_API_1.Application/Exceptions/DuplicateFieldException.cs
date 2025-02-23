using HITS_API_1.Domain.Exceptions;

namespace HITS_API_1.Application.Exceptions;

public class DuplicateFieldException : FieldException
{
    public DuplicateFieldException(String fieldName, String message) : base(fieldName, message) { }
}
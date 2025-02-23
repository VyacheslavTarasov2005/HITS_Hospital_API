using HITS_API_1.Domain.Exceptions;

namespace HITS_API_1.Application.Exceptions;

public class NotFoundObjectException : ObjectException
{
    public NotFoundObjectException(String objectName, String message) : base(objectName, message) { }
}
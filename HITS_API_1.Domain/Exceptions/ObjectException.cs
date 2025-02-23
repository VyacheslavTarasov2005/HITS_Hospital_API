namespace HITS_API_1.Domain.Exceptions;

public abstract class ObjectException : Exception
{
    public String ObjectName { get; }

    protected ObjectException(String objectName, String message) : base(message)
    {
        ObjectName = objectName;
    }
}
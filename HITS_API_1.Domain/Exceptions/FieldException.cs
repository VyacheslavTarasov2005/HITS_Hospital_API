namespace HITS_API_1.Domain.Exceptions;

public abstract class FieldException : Exception
{
    public String FieldName { get; }

    protected FieldException(String fieldName, String message) : base(message) 
    {
        FieldName = fieldName;
    }
}
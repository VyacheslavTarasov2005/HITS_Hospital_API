namespace HITS_API_1.Domain.Entities;

public class EmailMessage
{
    private Guid _id;
    private Guid _inspectionId;
    private String _name;
    private String _emailAddress;

    public Guid Id => _id;

    public Guid InspectionId => _inspectionId;

    public String Name => _name;

    public String EmailAddress => _emailAddress;

    public EmailMessage(Guid inspectionId, String name, String emailAddress)
    {
        _id = Guid.NewGuid();
        _inspectionId = inspectionId;
        _name = name;
        _emailAddress = emailAddress;
    }
}
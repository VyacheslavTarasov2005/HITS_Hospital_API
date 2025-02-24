namespace HITS_API_1.Domain.Entities;

public class Consultation
{
    private Guid _id;
    private DateTime _CreateTime;
    private Guid _inspectionId;
    private Guid _specialityId;

    public Guid Id => _id;

    public DateTime CreateTime => _CreateTime;

    public Guid InspectionId => _inspectionId;

    public Guid SpecialityId => _specialityId;

    public Consultation(Guid inspectionId, Guid specialityId)
    {
        _id = Guid.NewGuid();
        _CreateTime = DateTime.UtcNow;
        _inspectionId = inspectionId;
        _specialityId = specialityId;
    }
}
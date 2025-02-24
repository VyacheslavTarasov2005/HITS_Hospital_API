namespace HITS_API_1.Domain.Entities;

public class Diagnosis
{
    private Guid _id;
    private DateTime _createTime;
    private String? _description;
    private DiagnosisType _type;
    private Guid _inspectionId;
    private Guid _icd10Id;

    public Guid Id => _id;

    public DateTime CreateTime => _createTime;

    public String? Description
    {
        get => _description;
        set => _description = value;
    }

    public DiagnosisType Type
    {
        get => _type;
        set => _type = value;
    }

    public Guid InspectionId
    {
        get => _inspectionId;
    }

    public Guid Icd10Id => _icd10Id;

    public Diagnosis(String? description, DiagnosisType type, Guid inspectionId, Guid icd10Id)
    {
        _id = Guid.NewGuid();
        _createTime = DateTime.UtcNow;
        _description = description;
        _type = type;
        _inspectionId = inspectionId;
        _icd10Id = icd10Id;
    }
}
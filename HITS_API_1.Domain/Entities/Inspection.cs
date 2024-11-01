namespace HITS_API_1.Domain.Entities;

public class Inspection
{
    private Guid _id;
    private DateTime _createTime;
    private DateTime _date;
    private String _anamnesis;
    private String _complaints;
    private String _treatment;
    private Conclusion _conclusion;
    private DateTime? _nextVisitDate;
    private DateTime? _deathDate;
    private Guid? _previousInspectionId;
    private Guid _patientId;
    private Guid _doctorId;

    public Guid Id => _id;
    
    public DateTime CreateTime => _createTime;

    public DateTime Date
    {
        get => _date;
        set => _date = value;
    }

    public String Anamnesis
    {
        get => _anamnesis;
        set => _anamnesis = value;
    }

    public String Complaints
    {
        get => _complaints;
        set => _complaints = value;
    }

    public String Treatment
    {
        get => _treatment;
        set => _treatment = value;
    }

    public Conclusion Conclusion
    {
        get => _conclusion;
        set => _conclusion = value;
    }

    public DateTime? NextVisitDate
    {
        get => _nextVisitDate;
        set => _nextVisitDate = value;
    }

    public DateTime? DeathDate
    {
        get => _deathDate;
        set => _deathDate = value;
    }

    public Guid? PreviousInspectionId
    {
        get => _previousInspectionId;
        set => _previousInspectionId = value;
    }

    public Guid PatientId => _patientId;

    public Guid DoctorId => _doctorId;

    public Inspection(DateTime date, String anamnesis, String complaints, String treatment, Conclusion conclusion,
        DateTime? nextVisitDate, DateTime? deathDate, Guid? previousInspectionId, Guid patientId, Guid doctorId)
    {
        _id = Guid.NewGuid();
        _createTime = DateTime.UtcNow;
        _date = date;
        _anamnesis = anamnesis;
        _complaints = complaints;
        _treatment = treatment;
        _conclusion = conclusion;
        _nextVisitDate = nextVisitDate;
        _deathDate = deathDate;
        _previousInspectionId = previousInspectionId;
        _patientId = patientId;
        _doctorId = doctorId;
    }
}
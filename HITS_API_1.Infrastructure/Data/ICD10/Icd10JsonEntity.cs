namespace HITS_API_1.Infrastructure.Data.ICD10;

public class Icd10JsonEntity
{
    public int ID { get; set; }
    public String REC_CODE { get; set; }
    public String MKB_CODE { get; set; }
    public String MKB_NAME { get; set; }
    public String? ID_PARENT { get; set; }
    public int ACTUAL { get; set; }
}
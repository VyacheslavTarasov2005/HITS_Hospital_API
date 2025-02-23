namespace HITS_API_1.Entities;

public class ErrorResponse
{
    public String Type { get; set; }
    public int Status { get; set; }
    public Dictionary<String, List<String>> Errors { get; set; }

    public ErrorResponse(String type, int status, Dictionary<String, List<String>>? errors = null)
    {
        Type = type;
        Status = status;
        Errors = errors ?? new Dictionary<String, List<String>>();
    }

    public void AddError(String name, String description)
    {
        if (!Errors.ContainsKey(name))
        {
            Errors[name] = new List<String>();
        }
        
        Errors[name].Add(description);
    }
}
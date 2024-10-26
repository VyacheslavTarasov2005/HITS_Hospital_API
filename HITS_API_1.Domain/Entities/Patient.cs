namespace HITS_API_1.Domain.Entities;

public class Patient : Person
{
    public Patient(string name, DateTime? birthday, Gender sex) : base(name, birthday, sex) {}
}
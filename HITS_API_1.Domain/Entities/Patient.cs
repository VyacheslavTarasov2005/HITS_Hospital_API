namespace HITS_API_1.Domain.Entities;

public class Patient : Person
{
    public Patient(string name, DateTime birthday, Gender gender) : base(name, birthday, gender) {}
}
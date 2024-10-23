namespace HITS_API_1.Domain.Entities;

public class Patient : Person
{
    public Patient(string name, DateTime birthDate, Gender gender) : base(name, birthDate, gender) {}
}
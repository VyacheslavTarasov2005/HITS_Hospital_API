namespace HITS_API_1.Domain.Entities;

public class Patient(string name, DateTime? birthday, Gender sex) : Person(name, birthday, sex);
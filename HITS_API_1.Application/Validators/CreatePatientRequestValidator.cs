using FluentValidation;
using HITS_API_1.Application.DTOs;

namespace HITS_API_1.Application.Validators;

public class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(r => r.name)
            .NotEmpty()
            .WithMessage("Необходимо ФИО")
            .Length(1, 1000)
            .WithMessage("Допустимая длина ФИО - от 1 до 1000");
        
        RuleFor(r => r.birthday)
            .Must(birthday => birthday == null || birthday <= DateTime.UtcNow)
            .WithMessage("Дата рождения не может быть позже теккущей даты");
        
        RuleFor(r => r.gender)
            .NotNull()
            .WithMessage("Необходим пол")
            .IsInEnum()
            .WithMessage("Пол может принимать только значения Male и Female");
    }
}
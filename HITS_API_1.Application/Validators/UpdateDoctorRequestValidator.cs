using System.Text.RegularExpressions;
using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class UpdateDoctorRequestValidator : AbstractValidator<UpdateDoctorRequest>
{
    private readonly IDoctorsRepository _doctorsRepository;
    
    public UpdateDoctorRequestValidator(IDoctorsRepository doctorsRepository)
    {
        _doctorsRepository = doctorsRepository;

        RuleFor(r => r.email)
            .NotEmpty()
            .WithMessage("Необходим email")
            .EmailAddress()
            .WithMessage("email не соответствует требованиям email");

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
        
        RuleFor(r => r.phone)
            .Matches(new Regex(@"^\+7 \(\d{3}\) \d{3}-\d{2}-\d{2}$"))
            .When(r => r.phone != null)
            .WithMessage("Телефон должен соответствовать маске +7 (xxx) xxx-xx-xx");
    }
}
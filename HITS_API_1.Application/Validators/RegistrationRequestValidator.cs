using System.Text.RegularExpressions;
using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
{
    private readonly IDoctorsRepository _doctorsRepository;
    private readonly ISpecialitiesRepository _specialitiesRepository;

    public RegistrationRequestValidator(
        IDoctorsRepository doctorsRepository,
        ISpecialitiesRepository specialitiesRepository)
    {
        _doctorsRepository = doctorsRepository;
        _specialitiesRepository = specialitiesRepository;

        RuleFor(r => r.name)
            .NotEmpty()
            .WithMessage("Необходимо ФИО")
            .Length(1, 1000)
            .WithMessage("Допустимая длина ФИО - от 1 до 1000");

        RuleFor(r => r.password)
            .NotEmpty()
            .WithMessage("Необходим пароль")
            .MinimumLength(6)
            .WithMessage("Минимальная длина пароля - 6");

        RuleFor(r => r.email)
            .NotEmpty()
            .WithMessage("Необходим email")
            .MustAsync(async (email, CancellationToken) => await ValidateEmail(email))
            .WithMessage("email уже использован")
            .EmailAddress()
            .WithMessage("email не соответствует требованиям email");

        RuleFor(r => r.birthday)
            .Must(ValidateBirthday)
            .When(r => r.birthday != null)
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

        RuleFor(r => r.speciality)
            .NotEmpty()
            .WithMessage("Необходима специальность")
            .MustAsync(async (specialityId, CancellationToken) => await ValidateSpeciality(specialityId))
            .WithMessage("Специальность не существует");
    }

    private async Task<bool> ValidateEmail(string email)
    {
        var doctor = await _doctorsRepository.GetByEmail(email);

        if (doctor == null)
        {
            return true;
        }
        
        return false;
    }

    private bool ValidateBirthday(DateTime? birthday)
    {
        return birthday.Value < DateTime.UtcNow;
    }

    private async Task<bool> ValidateSpeciality(Guid specialityId)
    {
        var speciality = await _specialitiesRepository.GetById(specialityId);

        if (speciality == null)
        {
            return false;
        }
        
        return true;
    }
}
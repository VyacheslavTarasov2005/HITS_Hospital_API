using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.email)
            .NotEmpty()
            .WithMessage("Необходим email")
            .MinimumLength(1)
            .WithMessage("Минимальная длина email - 1");

        RuleFor(r => r.password)
            .NotEmpty()
            .WithMessage("Необходим пароль")
            .MinimumLength(1)
            .WithMessage("Минимальная длина пароля - 1");
    }
}
using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class CreateDiagnosisModelValidator : AbstractValidator<CreateDiagnosisModel>
{
    public CreateDiagnosisModelValidator()
    {
        RuleFor(r => r.icdDiagnosisId)
            .NotEmpty()
            .WithMessage("Необходим icdDiagnosisId");

        RuleFor(r => r.type)
            .NotNull()
            .WithMessage("Необходим тип диагноза")
            .IsInEnum()
            .WithMessage("Недопутимое значение типа диагноза");
    }
}
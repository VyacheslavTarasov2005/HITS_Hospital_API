using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class CreateDiagnosisModelValidator : AbstractValidator<CreateDiagnosisModel>
{
    private readonly IIcd10Repository _icd10Repository;
    
    public CreateDiagnosisModelValidator(IIcd10Repository icd10Repository)
    {
        _icd10Repository = icd10Repository;

        RuleFor(r => r.icdDiagnosisId)
            .MustAsync(async (icdDiagnosisId, CancellationToken) =>
                await ValidateIcdDiagnosisId(icdDiagnosisId))
            .WithMessage("Недопустимое значение icdDiagnosisId");

        RuleFor(r => r.type)
            .NotNull()
            .WithMessage("Необходим тип диагноза")
            .IsInEnum()
            .WithMessage("Недопутимое значение типа диагноза");
    }

    private async Task<bool> ValidateIcdDiagnosisId(Guid icdDiagnosisId)
    {
        var icd = await _icd10Repository.GetById(icdDiagnosisId);

        if (icd is null)
        {
            return false;
        }
        
        return true;
    }
}
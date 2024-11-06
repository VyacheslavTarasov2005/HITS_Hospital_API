using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class GetReportRequestValidator : AbstractValidator<GetReportRequest>
{
    private readonly IIcd10Repository _icd10Repository;
    
    public GetReportRequestValidator(IIcd10Repository icd10Repository)
    {
        _icd10Repository = icd10Repository;
        
        RuleFor(r => r.start)
            .NotEmpty()
            .WithMessage("Необходимо ввести начало временного интервала");

        RuleFor(r => r.end)
            .NotEmpty()
            .WithMessage("Необходимо ввести конец временного интервала");

        RuleFor(r => r)
            .Must(r => r.end >= r.start)
            .WithMessage("Некорректный временной интервал");
        
        RuleFor(r => r.icdRoots)
            .MustAsync(async (icdRoots, CancellationToken) => await ValidateIcd(icdRoots))
            .WithMessage("IcdRoots должны быть корневыми элементами МКБ");
    }
    
    private async Task<bool> ValidateIcd(List<Guid>? icdRoots)
    {
        if (icdRoots == null || icdRoots.Count == 0)
        {
            return true;
        }

        var roots = await _icd10Repository.GetRoots();

        foreach (var icdRoot in icdRoots)
        {
            if (roots.Find(r => r.Id == icdRoot) == null)
            {
                return false;
            }
        }

        return true;
    }
}
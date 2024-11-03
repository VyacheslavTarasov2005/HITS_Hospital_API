using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Validators;

public class GetFilteredInspectionsRequestValidator : AbstractValidator<GetFilteredInspectionsRequest>
{
    private readonly IIcd10Repository _icd10Repository;
    
    public GetFilteredInspectionsRequestValidator(IIcd10Repository icd10Repository)
    {
        _icd10Repository = icd10Repository;
        
        RuleFor(r => r.page)
            .GreaterThan(0)
            .When(r => r.page != null)
            .WithMessage("Номер страницы должен быть больше 0");
        
        RuleFor(r => r.size)
            .GreaterThan(0)
            .When(r => r.size != null)
            .WithMessage("Размер страницы должен быть больше 0");

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
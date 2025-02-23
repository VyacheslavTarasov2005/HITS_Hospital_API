using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class SpecialitiesService(
    ISpecialitiesRepository specialitiesRepository,
    IPaginationService paginationService,
    GetSpecialitiesRequestValidator getSpecialitiesRequestValidator)
    : ISpecialitiesService
{
    public async Task<(List<Speciality>, Pagination)> GetSpecialities(GetSpecialitiesRequest request)
    {
        var validationResult = await getSpecialitiesRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        
        var specialities = await specialitiesRepository.GetAllByNamePart(request.name ?? "");

        return paginationService.PaginateList(specialities, request.page, request.size);
    }
}
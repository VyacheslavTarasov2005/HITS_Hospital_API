using HITS_API_1.Application.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IPaginationService
{
    (List<T>, Pagination) PaginateList<T>(List<T> list, int? page, int? size);
}
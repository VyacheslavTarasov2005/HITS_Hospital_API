using HITS_API_1.Application.Entities;
using HITS_API_1.Application.Interfaces.Services;

namespace HITS_API_1.Application.Services;

public class PaginationService : IPaginationService
{
    public (List<T>, Pagination) PaginateList<T>(List<T> list, int? page, int? size)
    {
        Pagination pagination = new Pagination(size, list.Count, page);
        
        if (pagination.Count == 0)
        {
            return (list, pagination);
        }

        int startIndex = pagination.Size * (pagination.Current - 1);
        
        if (startIndex + 1 > list.Count)
        {
            throw new ArgumentException("Недопустимое значение номера страницы");
        }

        int endIndex = int.Min(pagination.Size * pagination.Current, list.Count);
        
        List<T> paginatedList = new List<T>();
        
        for (int i = startIndex; i < endIndex; i++)
        {
            paginatedList.Add(list[i]);
        }
        
        return (paginatedList, pagination);
    }
}
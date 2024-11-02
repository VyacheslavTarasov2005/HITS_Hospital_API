using HITS_API_1.Application.Entities;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetPatientsListRequest(
    String? name,
    Conclusion? conclusions,
    Sorting? sorting,
    bool? scheduledVisits,
    bool? onlyMine,
    int? page,
    int? size);
using HITS_API_1.Application.Entities;
using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetSpecialitiesResponse(
    List<Speciality> specialities,
    Pagination pagination);
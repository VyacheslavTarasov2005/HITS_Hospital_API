namespace HITS_API_1.Application.DTOs;

public record GetSpecialitiesRequest(
    String? name,
    Int32 page,
    Int32 size);
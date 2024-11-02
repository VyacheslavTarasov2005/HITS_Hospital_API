namespace HITS_API_1.Application.DTOs;

public record GetSpecialitiesRequest(
    String? name,
    int? page,
    int? size);
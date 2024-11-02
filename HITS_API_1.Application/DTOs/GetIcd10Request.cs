namespace HITS_API_1.Application.DTOs;

public record GetIcd10Request(
    String? request,
    int? page,
    int? size);
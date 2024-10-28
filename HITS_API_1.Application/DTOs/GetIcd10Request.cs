namespace HITS_API_1.Application.DTOs;

public record GetIcd10Request(
    String? request,
    Int32 page,
    Int32 size);
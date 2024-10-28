using HITS_API_1.Application.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetIcd10Response(
    List<Icd10Response> records,
    Pagination pagination);
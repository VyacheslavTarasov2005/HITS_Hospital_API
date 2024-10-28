namespace HITS_API_1.Application.DTOs;

public record Icd10Response(
    String code,
    String name,
    Guid id,
    DateTime createTime);
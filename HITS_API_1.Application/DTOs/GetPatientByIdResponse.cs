using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetPatientByIdResponse(
    Guid id,
    DateTime createTime,
    String name,
    DateTime? birthday,
    Gender gender);
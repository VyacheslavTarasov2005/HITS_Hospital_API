using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetDoctorResponse(
    Guid id,
    DateTime creaateTime,
    String name,
    DateTime? birthday,
    Gender gender,
    String email,
    String? phone);
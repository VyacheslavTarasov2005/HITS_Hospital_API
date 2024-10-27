using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record UpdateDoctorRequest(
    String email,
    String name,
    DateTime? birthday,
    Gender gender,
    String? phone);
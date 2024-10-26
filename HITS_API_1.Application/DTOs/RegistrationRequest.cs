using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record RegistrationRequest(
    String name,
    String password,
    String email,
    DateTime? birthday,
    Gender gender,
    String? phone,
    Guid speciality);
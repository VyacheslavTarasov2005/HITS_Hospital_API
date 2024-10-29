using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record CreatePatientRequest(
    String name,
    DateTime? birthday,
    Gender gender);
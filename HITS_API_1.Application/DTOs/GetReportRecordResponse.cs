using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.DTOs;

public record GetReportRecordResponse(
    String patientName,
    DateTime? patientBirthdate,
    Gender gender,
    Dictionary<String, int>? visitsByRoot);
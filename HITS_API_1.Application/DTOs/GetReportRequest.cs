namespace HITS_API_1.Application.DTOs;

public record GetReportRequest(
    DateTime start,
    DateTime end,
    List<Guid>? icdRoots);
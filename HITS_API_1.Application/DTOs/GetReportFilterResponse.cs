namespace HITS_API_1.Application.DTOs;

public record GetReportFilterResponse(
    DateTime start,
    DateTime end,
    List<String> icdRoots);
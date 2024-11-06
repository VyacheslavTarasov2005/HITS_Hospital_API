namespace HITS_API_1.Application.DTOs;

public record GetReportResponse(
    GetReportFilterResponse filters,
    List<GetReportRecordResponse>? records,
    Dictionary<String, int>? summaryByRoot);
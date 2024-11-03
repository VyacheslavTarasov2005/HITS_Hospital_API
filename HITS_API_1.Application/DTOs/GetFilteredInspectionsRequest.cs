namespace HITS_API_1.Application.DTOs;

public record GetFilteredInspectionsRequest(
    bool? grouped,
    List<Guid>? icdRoots,
    int? page,
    int? size);
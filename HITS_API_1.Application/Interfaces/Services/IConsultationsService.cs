using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Application.Interfaces.Services;

public interface IConsultationsService
{
    Task<Guid> CreateConsultation(Guid inspectionId, Guid specialityId, Guid doctorId, String commentContent);
    Task<(Consultation?, List<Comment>)> GetConsultationById(Guid consultationId);
}
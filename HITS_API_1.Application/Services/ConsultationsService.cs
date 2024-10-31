using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class ConsultationsService : IConsultationsService
{
    private readonly IConsultationsRepository _consultationsRepository;
    private readonly ICommentsRepository _commentsRepository;

    public ConsultationsService(IConsultationsRepository consultationsRepository,
        ICommentsRepository commentsRepository)
    {
        _consultationsRepository = consultationsRepository;
        _commentsRepository = commentsRepository;
    }

    public async Task<Guid> CreateConsultation(Guid inspectionId, Guid specialityId, Guid doctorId,
        String commentContent)
    {
        Consultation consultation = new Consultation(inspectionId, specialityId);
        
        await _consultationsRepository.Create(consultation);
        
        Comment comment = new Comment(null, commentContent, doctorId, null, consultation.Id);
        
        await _commentsRepository.Create(comment);
        
        return consultation.Id;
    }
}
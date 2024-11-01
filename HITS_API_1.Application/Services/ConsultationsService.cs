using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class ConsultationsService : IConsultationsService
{
    private readonly IConsultationsRepository _consultationsRepository;
    private readonly ICommentsRepository _commentsRepository;
    private readonly IDoctorsService _doctorsService;
    private readonly ISpecialitiesRepository _specialitiesRepository;

    public ConsultationsService(
        IConsultationsRepository consultationsRepository,
        ICommentsRepository commentsRepository,
        IDoctorsService doctorsService,
        ISpecialitiesRepository specialitiesRepository)
    {
        _consultationsRepository = consultationsRepository;
        _commentsRepository = commentsRepository;
        _doctorsService = doctorsService;
        _specialitiesRepository = specialitiesRepository;
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

    public async Task<(Consultation?, List<Comment>)> GetConsultationById(Guid consultationId)
    {
        var consultation = await _consultationsRepository.GetById(consultationId);

        if (consultation == null)
        {
            return (null, []);
        }
        
        var comments = await _commentsRepository.GetByConsultationId(consultationId);

        return (consultation, comments);
    }

    public async Task<List<InspectionConsultationModel>> GetAllConsultationsByInspection(Guid inspectionId)
    {
        var consultations = await _consultationsRepository.GetAllByInspectionId(inspectionId);
        
        List<InspectionConsultationModel> consultationResponse = new List<InspectionConsultationModel>();

        foreach (var consultation in consultations)
        {
            var speciality = await _specialitiesRepository.GetById(consultation.SpecialityId);
                
            var comments = await _commentsRepository.GetByConsultationId(consultation.Id);

            var rootComment = comments.MinBy(c => c.CreateTime);
        
            var rootCommentAuthor = await _doctorsService.GetDoctor(rootComment.AuthorId);
        
            int commentsNumber = comments.Count();
            
            GetDoctorResponse rootCommentAuthorResponse = new GetDoctorResponse(rootCommentAuthor.Id,
                rootCommentAuthor.CreateTime, rootCommentAuthor.Name, rootCommentAuthor.Birthday, rootCommentAuthor.Sex,
                rootCommentAuthor.Email, rootCommentAuthor.Phone);
            
            InspectionCommentModel commentResponse = new InspectionCommentModel(rootComment.Id, rootComment.CreateTime,
                rootComment.ParentId, rootComment.Content, rootCommentAuthorResponse,
                rootComment.ModifiedDate ?? rootComment.CreateTime);
            
            InspectionConsultationModel response = new InspectionConsultationModel(consultation.Id, 
                consultation.CreateTime, inspectionId, speciality, commentResponse, commentsNumber);
            
            consultationResponse.Add(response);
        }
        
        return consultationResponse;
    }
}
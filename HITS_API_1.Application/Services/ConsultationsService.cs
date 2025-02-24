using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Exceptions;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Exceptions;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class ConsultationsService(
    IConsultationsRepository consultationsRepository,
    ICommentsRepository commentsRepository,
    IDoctorsService doctorsService,
    ISpecialitiesRepository specialitiesRepository)
    : IConsultationsService
{
    public async Task<Guid> CreateConsultation(Guid inspectionId, Guid specialityId, Guid doctorId,
        String commentContent)
    {
        Consultation consultation = new Consultation(inspectionId, specialityId);

        await consultationsRepository.Create(consultation);

        Comment comment = new Comment(null, commentContent, doctorId, null, consultation.Id);

        await commentsRepository.Create(comment);

        return consultation.Id;
    }

    public async Task<(Consultation, List<Comment>, Speciality)> GetConsultationById(Guid consultationId)
    {
        var consultation = await consultationsRepository.GetById(consultationId);
        if (consultation == null)
        {
            throw new NotFoundObjectException("consultation", "Консультация не найдена");
        }

        var speciality = await specialitiesRepository.GetById(consultation.SpecialityId);

        var comments = await commentsRepository.GetByConsultationId(consultationId);

        return (consultation, comments, speciality);
    }

    public async Task<List<InspectionConsultationModel>> GetAllConsultationsByInspection(Guid inspectionId)
    {
        var consultations = await consultationsRepository.GetAllByInspectionId(inspectionId);

        List<InspectionConsultationModel> consultationResponse = new List<InspectionConsultationModel>();

        foreach (var consultation in consultations)
        {
            var speciality = await specialitiesRepository.GetById(consultation.SpecialityId);

            var comments = await commentsRepository.GetByConsultationId(consultation.Id);

            var rootComment = comments.MinBy(c => c.CreateTime);

            var rootCommentAuthor = await doctorsService.GetDoctor(rootComment.AuthorId);

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

    public async Task ValidateConsultations(List<CreateConsultationModel> consultations)
    {
        if (consultations.Count > 0)
        {
            foreach (var consultation in consultations)
            {
                var speciality = await specialitiesRepository.GetById(consultation.specialityId);
                if (speciality == null)
                {
                    throw new IncorrectFieldException("consultations/specialityId",
                        $"Специальность {consultation.specialityId} не найдена");
                }
            }
        }
    }
}
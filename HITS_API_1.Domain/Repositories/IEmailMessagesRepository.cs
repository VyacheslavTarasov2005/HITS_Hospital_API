using HITS_API_1.Domain.Entities;

namespace HITS_API_1.Domain.Repositories;

public interface IEmailMessagesRepository
{
    Task<EmailMessage?> GetByInspectionId(Guid inspectionId);
    Task Add(EmailMessage email);
}
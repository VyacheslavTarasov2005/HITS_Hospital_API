using System.Net.Mail;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class EmailsService : IEmailsService
{
    private readonly String _smtpServer = "localhost";
    private readonly int _smtpPort = 1025;
    
    private readonly IEmailMessagesRepository _emailMessagesRepository;
    private readonly IPatientsRepository _patientsRepository;
    private readonly IInspectionsRepository _inspectionsRepository;
    private readonly IDoctorsRepository _doctorsRepository;

    public EmailsService(
        IEmailMessagesRepository emailMessagesRepository,
        IPatientsRepository patientsRepository,
        IInspectionsRepository inspectionsRepository,
        IDoctorsRepository doctorsRepository)
    {
        _emailMessagesRepository = emailMessagesRepository;
        _patientsRepository = patientsRepository;
        _inspectionsRepository = inspectionsRepository;
        _doctorsRepository = doctorsRepository;
    }

    public async Task CheckInspecions()
    {
        var patients = await _patientsRepository.GetAllByNamePart("");

        foreach (var patient in patients)
        {
            var inspections = await _inspectionsRepository.GetAllByPatientId(patient.Id);
            inspections = inspections
                .Where(i => i.NextVisitDate != null && i.NextVisitDate < DateTime.UtcNow)
                .ToList();

            foreach (var inspection in inspections)
            {
                var childInspection = await _inspectionsRepository.GetByParentInspectionId(inspection.Id);

                if (childInspection == null)
                {
                    var email = await _emailMessagesRepository.GetByInspectionId(inspection.Id);
                    if (email == null)
                    {
                        var inspectionAuthor = await _doctorsRepository.GetById(inspection.DoctorId);
                    
                        try
                        {
                            await SendEmail(patient.Name, inspection.NextVisitDate.Value, inspectionAuthor.Email);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    
                        EmailMessage emailEntity = new EmailMessage(inspection.Id, patient.Name, 
                            inspectionAuthor.Email);
                        
                        await _emailMessagesRepository.Add(emailEntity);
                    }
                }
            }
        }
    }

    private async Task SendEmail(String patientName, DateTime inspectionDate, String doctorEmail)
    {
        var smtpClient = new SmtpClient(_smtpServer, _smtpPort);

        var emailMessage = new MailMessage
        {
            From = new MailAddress("emailsender@example.com"),
            Subject = "Пропущенный визит",
            Body = $"Пациент {patientName} пропустил осмотр, назначенный на {inspectionDate:dd/MM/yyyy}"
        };
        
        emailMessage.To.Add($"{doctorEmail}");
        
        await smtpClient.SendMailAsync(emailMessage);
    } 
}
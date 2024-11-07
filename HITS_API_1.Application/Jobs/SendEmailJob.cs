using HITS_API_1.Application.Interfaces.Services;
using Quartz;

namespace HITS_API_1.Application.Jobs;

public class SendEmailJob : IJob
{
    private readonly IEmailsService _emailsService;

    public SendEmailJob(IEmailsService emailsService)
    {
        _emailsService = emailsService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _emailsService.CheckInspecions();
    }
}
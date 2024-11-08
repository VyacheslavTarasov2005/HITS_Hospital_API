using HITS_API_1.Application.Interfaces.Services;
using Quartz;

namespace HITS_API_1.Application.Jobs;

public class SendEmailJob(IEmailsService emailsService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await emailsService.CheckInspecions();
    }
}
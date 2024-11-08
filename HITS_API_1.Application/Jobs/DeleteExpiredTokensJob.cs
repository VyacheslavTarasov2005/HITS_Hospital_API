using HITS_API_1.Application.Interfaces.Services;
using Quartz;

namespace HITS_API_1.Application.Jobs;

public class DeleteExpiredTokensJob(ITokensService tokensService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await tokensService.DeleteExpiredTokens();
    }
}
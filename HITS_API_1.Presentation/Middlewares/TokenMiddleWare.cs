using HITS_API_1.Application.Services;
using Microsoft.AspNetCore.Authorization;
namespace HITS_API_1.Middlewares;

public class TokenMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly TokensService _iTokensService;

    public TokenMiddleWare(RequestDelegate next, TokensService iTokensService)
    {
        _next = next;
        _iTokensService = iTokensService;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        var endpoint = httpContext.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
        {
            await _next(httpContext);
            return;
        }
        
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (token == null)
        {
            httpContext.Response.StatusCode = 401;
            return;
        }
        
        var dbToken = await _iTokensService.GetToken(token);

        if (dbToken == null || dbToken.ExpiryDate < DateTime.UtcNow)
        {
            httpContext.Response.StatusCode = 401;
            return;
        }
        
        await _next(httpContext);
    }
}
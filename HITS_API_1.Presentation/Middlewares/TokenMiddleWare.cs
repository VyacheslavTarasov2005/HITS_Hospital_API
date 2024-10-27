using HITS_API_1.Domain;
using Microsoft.AspNetCore.Authorization;
namespace HITS_API_1.Middlewares;

public class TokenMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ITokensService _tokensService;

    public TokenMiddleWare(RequestDelegate next, ITokensService tokensService)
    {
        _next = next;
        _tokensService = tokensService;
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
        
        var dbToken = await _tokensService.GetToken(token);

        if (dbToken == null)
        {
            httpContext.Response.StatusCode = 401;
            return;
        }

        if (dbToken.ExpiryDate < DateTime.UtcNow)
        {
            _tokensService.DeleteToken(token);
            httpContext.Response.StatusCode = 401;
            return;
        }
        
        await _next(httpContext);
    }
}
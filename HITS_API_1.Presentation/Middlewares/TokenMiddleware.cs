using System.Security.Claims;
using HITS_API_1.Application.Interfaces;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Domain;
using Microsoft.AspNetCore.Authorization;
namespace HITS_API_1.Middlewares;

public class TokenMiddleware
{
    private readonly RequestDelegate _next;
    public TokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, ITokensService tokensService, IHasher hasher)
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
        
        var dbToken = await tokensService.GetToken(token);

        if (dbToken == null)
        {
            httpContext.Response.StatusCode = 401;
            return;
        }

        if (dbToken.ExpiryDate < DateTime.UtcNow)
        {
            await tokensService.DeleteToken(token);
            httpContext.Response.StatusCode = 401;
            return;
        }
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, dbToken.Doctor.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Token");
        httpContext.User = new ClaimsPrincipal(identity);
        await _next(httpContext);
    }
}
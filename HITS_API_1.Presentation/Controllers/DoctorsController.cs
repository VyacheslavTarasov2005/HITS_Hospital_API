using System.Security.Claims;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/doctor")]
public class DoctorsController(
    IDoctorsService doctorsService)
    : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> RegisterDoctor([FromBody] RegistrationRequest request)
    {
        String accesToken = await doctorsService.RegisterDoctor(request);

        var response = new AuthenticationResponse(accesToken);
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> LoginDoctor([FromBody] LoginRequest request)
    {
        var accesToken = await doctorsService.LoginDoctor(request);

        var response = new AuthenticationResponse(accesToken);
        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> LogoutDoctor()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        await doctorsService.LogoutDoctor(token);

        return Ok();
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<GetDoctorResponse>> GetDoctor()
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            throw new UnauthorizedAccessException();
        }

        var doctor = await doctorsService.GetDoctor(Guid.Parse(doctorId));

        var response = new GetDoctorResponse(doctor.Id, doctor.CreateTime, doctor.Name, doctor.Birthday,
            doctor.Sex, doctor.Email, doctor.Phone);
        return Ok(response);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult> UpdateDoctor([FromBody] UpdateDoctorRequest request)
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            throw new UnauthorizedAccessException();
        }

        await doctorsService.UpdateDoctor(Guid.Parse(doctorId), request);
        return Ok();
    }
}
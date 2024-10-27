using System.Security.Claims;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Domain;
using HITS_API_1.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HITS_API_1.Controllers;

[ApiController]
[Route("api/doctor")]
public class DoctorsController : ControllerBase
{
    private readonly IDoctorsService _doctorsService;

    public DoctorsController(IDoctorsService doctorsService)
    {
        _doctorsService = doctorsService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> RegisterDoctor([FromBody] RegistrationRequest request)
    {
        Doctor doctor = new Doctor(request.name, request.birthday, request.gender, request.phone, request.email,
            request.password, request.speciality);

        String accesToken = await _doctorsService.RegisterDoctor(doctor);
        
        AuthenticationResponse response = new AuthenticationResponse(accesToken);
        
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> LoginDoctor([FromBody] LoginRequest request)
    {
        var accesToken = await _doctorsService.LoginDoctor(request.email, request.password);

        if (accesToken == null)
        {
            return BadRequest();
        }
        
        return Ok(new AuthenticationResponse(accesToken));
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<GetDoctorResponse>> GetDoctor()
    {
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            return NotFound();
        }
        
        var doctor = await _doctorsService.GetDoctor(Guid.Parse(doctorId));
        if (doctor == null)
        {
            return NotFound();
        }
        
        return Ok(new GetDoctorResponse(doctor.Id, doctor.CreateTime, doctor.Name, doctor.Birthday, doctor.Sex, 
            doctor.Email, doctor.Phone));
    }
}
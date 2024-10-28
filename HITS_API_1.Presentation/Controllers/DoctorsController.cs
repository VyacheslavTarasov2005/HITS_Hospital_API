using System.Security.Claims;
using FluentValidation;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Interfaces.Services;
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
    private readonly IValidator<RegistrationRequest> _registrationRequestValidator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;
    private readonly IValidator<UpdateDoctorRequest> _updateDoctorRequestValidator;

    public DoctorsController(
        IDoctorsService doctorsService, 
        IValidator<RegistrationRequest> registrationRequestValidator,
        IValidator<LoginRequest> loginRequestValidator,
        IValidator<UpdateDoctorRequest> updateDoctorRequestValidator)
    {
        _doctorsService = doctorsService;
        _registrationRequestValidator = registrationRequestValidator;
        _loginRequestValidator = loginRequestValidator;
        _updateDoctorRequestValidator = updateDoctorRequestValidator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthenticationResponse>> RegisterDoctor([FromBody] RegistrationRequest request)
    {
        var validationResult = await _registrationRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
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
        var validationResult = await _loginRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var accesToken = await _doctorsService.LoginDoctor(request.email, request.password);

        if (accesToken == null)
        {
            return BadRequest("Пользователь с такими данными не найдем");
        }
        
        AuthenticationResponse response = new AuthenticationResponse(accesToken);
        
        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> LogoutDoctor()
    {
        var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        await _doctorsService.LogoutDoctor(token);
        
        return Ok();
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

        GetDoctorResponse response = new GetDoctorResponse(doctor.Id, doctor.CreateTime, doctor.Name, doctor.Birthday,
            doctor.Sex, doctor.Email, doctor.Phone);
        
        return Ok(response);
    }

    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult> UpdateDoctor([FromBody] UpdateDoctorRequest request)
    {
        var validationResult = await _updateDoctorRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        
        var doctorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (doctorId == null)
        {
            return NotFound();
        }

        try
        {
            await _doctorsService.UpdateDoctor(Guid.Parse(doctorId), request.email, request.name, request.birthday,
                request.gender, request.phone);
            
            return Ok();
        }
        catch (Exception e)
        {
            if (e.Message == "Пользователь не найден")
            {
                return NotFound();
            }
            
            return BadRequest("email уже использован");
        }
    }
}
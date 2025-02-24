using System.Security.Authentication;
using HITS_API_1.Application.DTOs;
using HITS_API_1.Application.Exceptions;
using HITS_API_1.Application.Interfaces;
using HITS_API_1.Application.Interfaces.Services;
using HITS_API_1.Application.Validators;
using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;

namespace HITS_API_1.Application.Services;

public class DoctorsService(
    IDoctorsRepository doctorsRepository,
    ISpecialitiesRepository specialitiesRepository,
    ITokensService tokensService,
    IHasher hasher,
    RegistrationRequestValidator registrationRequestValidator,
    LoginRequestValidator loginRequestValidator,
    UpdateDoctorRequestValidator updateDoctorRequestValidator)
    : IDoctorsService
{
    public async Task<String> RegisterDoctor(RegistrationRequest request)
    {
        var validationResult = await registrationRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var speciality = await specialitiesRepository.GetById(request.speciality);
        if (speciality == null)
        {
            throw new NotFoundFieldException("speciality", "Специальность не найдена");
        }

        var duplicateEmailDoctor = await doctorsRepository.GetByEmail(request.email);
        if (duplicateEmailDoctor != null)
        {
            throw new DuplicateFieldException("email", "Email уже используется другим пользователем");
        }

        Doctor doctor = new Doctor(request.name, request.birthday, request.gender, request.phone, request.email,
            request.password, request.speciality);

        doctor.Password = hasher.Hash(doctor.Password);
        var doctorId = await doctorsRepository.Create(doctor);

        return await tokensService.CreateToken(doctorId);
    }

    public async Task<String> LoginDoctor(LoginRequest request)
    {
        var validationResult = await loginRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var doctor = await doctorsRepository.GetByEmail(request.email);
        if (doctor == null)
        {
            throw new InvalidCredentialException();
        }

        if (hasher.Verify(request.password, doctor.Password))
        {
            return await tokensService.CreateToken(doctor.Id);
        }

        throw new InvalidCredentialException();
    }

    public async Task LogoutDoctor(String token)
    {
        await tokensService.DeleteToken(token);
    }

    public async Task<Doctor> GetDoctor(Guid id)
    {
        var doctor = await doctorsRepository.GetById(id);

        if (doctor == null)
        {
            throw new NotFoundObjectException("user", "Пользователь не найден");
        }

        return doctor;
    }

    public async Task UpdateDoctor(Guid id, UpdateDoctorRequest request)
    {
        var validationResult = await updateDoctorRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var doctor = await doctorsRepository.GetById(id);
        if (doctor == null)
        {
            throw new NotFoundObjectException("user", "Пользователь не найден");
        }

        var sameEmailDoctor = await doctorsRepository.GetByEmail(request.email);
        if (sameEmailDoctor != null && sameEmailDoctor.Id != id)
        {
            throw new DuplicateFieldException("email", "Email уже используется другим пользоавтелем");
        }

        await doctorsRepository.Update(id, request.email, request.name, request.birthday, request.gender,
            request.phone);
    }
}
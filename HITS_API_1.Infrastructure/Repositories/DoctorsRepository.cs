using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class DoctorsRepository(ApplicationDbContext dbContext) : IDoctorsRepository
{
    public async Task<Guid> Create(Doctor doctor)
    {
        await dbContext.Doctors.AddAsync(doctor);
        await dbContext.SaveChangesAsync();
        
        return doctor.Id;
    }

    public async Task<Doctor?> GetById(Guid id)
    {
        var doctor = await dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == id);
        
        return doctor;
    }

    public async Task<Doctor?> GetByEmail(String email)
    {
        var doctor = await dbContext.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Email == email);
        
        return doctor;
    }

    public async Task<List<Doctor>> GetAllByEmail(String email)
    {
        var doctors = await dbContext.Doctors
            .AsNoTracking()
            .Where(d => d.Email == email)
            .ToListAsync();
        
        return doctors;
    }

    public async Task<Guid> Update(Guid id, String email, String name, DateTime? birthday, Gender gender, String? phoneNumber)
    {
        await dbContext.Doctors
            .Where(d => d.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(d => d.Email, d => email)
                .SetProperty(d => d.Name, d => name)
                .SetProperty(d => d.Birthday, d => birthday ?? d.Birthday)
                .SetProperty(d => d.Sex, d => gender)
                .SetProperty(d => d.Phone, d => phoneNumber ?? d.Phone));
        
        await dbContext.SaveChangesAsync();
        
        return id;
    }
}
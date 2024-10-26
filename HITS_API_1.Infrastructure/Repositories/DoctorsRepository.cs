using HITS_API_1.Domain.Entities;
using HITS_API_1.Domain.Repositories;
using HITS_API_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HITS_API_1.Infrastructure.Repositories;

public class DoctorsRepository : IDoctorsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public DoctorsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Create(Doctor doctor)
    {
        await _dbContext.Doctors.AddAsync(doctor);
        await _dbContext.SaveChangesAsync();
        
        return doctor.Id;
    }

    public async Task<Doctor?> Get(Guid id)
    {
        var doctor = await _dbContext.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
        
        return doctor;
    }

    public async Task<Guid> Update(Guid id, String email, String name, DateTime birthday, Gender gender, String phoneNumber)
    {
        await _dbContext.Doctors
            .Where(d => d.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(d => d.Email, d => email)
                .SetProperty(d => d.Name, d => name)
                .SetProperty(d => d.Birthday, d => birthday)
                .SetProperty(d => d.Sex, d => gender)
                .SetProperty(d => d.Phone, d => phoneNumber));
        
        return id;
    }
}
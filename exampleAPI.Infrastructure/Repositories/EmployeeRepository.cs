using Microsoft.EntityFrameworkCore;
using MiApi.Domain.Entities;
using MiApi.Domain.Interfaces;
using MiApi.Infrastructure.Data;

namespace MiApi.Infrastructure.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Username == username, ct);
    }

    public async Task<Employee?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Email == email, ct);
    }

    public async Task<IReadOnlyList<Employee>> GetByCompanyIdAsync(int companyId, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(e => e.CompanyId == companyId)
            .ToListAsync(ct);
    }

    public override async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _dbSet.FindAsync([id], cancellationToken: ct);
        if (entity is null)
            return false;

        _dbSet.Remove(entity);
        return true;
    }
}
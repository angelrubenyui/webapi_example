using System.Linq.Expressions;
using MiApi.Domain.Entities;

namespace MiApi.Domain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee?> GetByUsernameAsync(string username, CancellationToken ct = default);
        Task<Employee?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IReadOnlyList<Employee>> GetByCompanyIdAsync(int companyId, CancellationToken ct = default);
    }
}

using System.Linq.Expressions;

namespace MiApi.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<T> AddAsync(T entity, CancellationToken ct = default);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

}
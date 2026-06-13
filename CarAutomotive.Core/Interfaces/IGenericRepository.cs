using CarAutomotive.Core.Common;

namespace CarAutomotive.Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(int id);

        Task<IReadOnlyList<T>> GetAllAsync();

        Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec);

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);

        Task<int> CountAsync(ISpecification<T> spec);

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);
        // Backward compatibility for existing dev code / mechanic module
        Task<IReadOnlyList<T>> GetAll();

        Task<T?> GetById(Guid id);

        Task<T?> GetEntityWithSpec(ISpecification<T> spec);

        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<IReadOnlyList<T>> GetAllWithSpecTrackedAsync(ISpecification<T> spec);
    }
}
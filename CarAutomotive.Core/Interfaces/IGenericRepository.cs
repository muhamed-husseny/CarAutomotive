using CarAutomotive.Core.Common;

namespace CarAutomotive.Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T?> GetById(Guid id);
        Task<IReadOnlyList<T>> GetAll();
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}

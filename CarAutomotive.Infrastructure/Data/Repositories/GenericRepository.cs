using CarAutomotive.Core.Common;
using CarAutomotive.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarAutomotive.Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec)
        {
            return await SpecificationEvaluator<T>
                .GetQuery(_context.Set<T>(), spec)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await SpecificationEvaluator<T>
                .GetQuery(_context.Set<T>(), spec)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await SpecificationEvaluator<T>
                .GetQuery(_context.Set<T>(), spec)
                .CountAsync();
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public async Task<IReadOnlyList<T>> GetAll()
        {
            return await GetAllAsync();
        }

        public async Task<T?> GetById(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetEntityWithSpec(ISpecification<T> spec)
        {
            return await GetByIdWithSpecAsync(spec);
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await GetAllWithSpecAsync(spec);
        }
    }
}
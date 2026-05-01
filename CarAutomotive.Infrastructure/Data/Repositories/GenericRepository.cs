namespace CarAutomotive.Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public GenericRepository(ApplicationDbContext context) // ask clr to create object from AppdbContext 
        {
            _context = context;
        }
        //specification design pattern : to separate the logic of querying data from the logic of business rules and other concerns, by encapsulating the query logic in a separate class called a specification and make the query -> "DYNIMCAL".

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int Id)
        {
            return await _context.Set<T>().FindAsync(Id);
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

        public async Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec)
        {
            return await SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec).FirstOrDefaultAsync();
        }
        public async Task<int> CountAsync(ISpecification<T> spec)
        {
            return await SpecificationEvaluator<T>
                        .GetQuery(_context.Set<T>(), spec)
                        .CountAsync();
        }
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec).AsNoTracking().ToListAsync();
        }

     
    }
}

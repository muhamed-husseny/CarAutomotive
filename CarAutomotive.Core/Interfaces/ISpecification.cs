namespace CarAutomotive.Core.Interfaces
{
    public class ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; }
        public Expression<Func<T, object>> OrderBy { get; }
        public Expression<Func<T, object>> OrderByDescending { get; }
    }
}

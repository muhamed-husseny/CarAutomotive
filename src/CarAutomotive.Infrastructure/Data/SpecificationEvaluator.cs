namespace CarAutomotive.Infrastructure.Data
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(
            IQueryable<T> inputQuery,
            ISpecification<T> spec)
        {
            var query = inputQuery;

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            if (spec.IsPagingEnabled)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            if (spec.Includes.Any())
            {
                query = spec.Includes.Aggregate(
                    query,
                    (current, include) => current.Include(include));
            }

            if (spec.IncludeStrings.Any())
            {
                query = spec.IncludeStrings.Aggregate(
                    query,
                    (current, include) => current.Include(include));
            }

            return query;
        }
    }
}
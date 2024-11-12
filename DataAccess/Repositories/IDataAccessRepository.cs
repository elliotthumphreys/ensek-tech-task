using System.Linq.Expressions;

namespace DataAccess.Repositories;

public interface IDataAccessRepository<T> where T : class
{
    IQueryable<T> Get(Expression<Func<T, bool>> predicate);

    T Add(T entity);
    T Update(T entity);

}

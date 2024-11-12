using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess.Repositories;

internal class GenericDataAccessRepository<T> : IDataAccessRepository<T> where T : class
{
    private readonly MeterReadingsDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericDataAccessRepository(MeterReadingsDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> Get(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public T Add(T entity)
    {
        return _dbSet.Add(entity).Entity;
    }

    public T Update(T entity)
    {
        return _dbSet.Update(entity).Entity;
    }
}

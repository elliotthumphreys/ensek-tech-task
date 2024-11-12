namespace DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MeterReadingsDbContext _context;

    public UnitOfWork(MeterReadingsDbContext context)
    {
        _context = context;
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }
}
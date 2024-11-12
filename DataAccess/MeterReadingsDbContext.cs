using DataAccess.MappingConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccess;

public class MeterReadingsDbContext : DbContext
{
    public MeterReadingsDbContext(DbContextOptions<MeterReadingsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new AccountMappings());
        modelBuilder.ApplyConfiguration(new MeterReadingMappings());
    }
}

// used only for efc migrations
internal class MeterReadingsDbContextMigrationsFactory : IDesignTimeDbContextFactory<MeterReadingsDbContext>
{
    public MeterReadingsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MeterReadingsDbContext>();

        optionsBuilder.UseSqlServer(string.Empty);

        return new MeterReadingsDbContext(optionsBuilder.Options);
    }
}
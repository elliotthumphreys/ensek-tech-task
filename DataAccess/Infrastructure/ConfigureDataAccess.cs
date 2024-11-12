using DataAccess.Repositories;
using Domain;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Infrastructure;

public static class ConfigureDataAccess
{
    public static void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        serviceCollection.AddScoped<IDataAccessRepository<Account>, GenericDataAccessRepository<Account>>();
        serviceCollection.AddScoped<IDataAccessRepository<MeterReading>, GenericDataAccessRepository<MeterReading>>();
    }
}
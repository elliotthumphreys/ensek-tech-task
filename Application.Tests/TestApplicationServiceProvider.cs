using Application.Infrastructure;
using DataAccess.Infrastructure;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Tests;

public class TestApplicationServiceProvider
{
    public readonly IServiceProvider ServiceProvider;

    public TestApplicationServiceProvider()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddDbContext<MeterReadingsDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        ConfigureDataAccess.ConfigureServices(serviceCollection);
        ConfigureApplication.ConfigureServices(serviceCollection);

        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    public T GetSut<T>() where T : class
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}


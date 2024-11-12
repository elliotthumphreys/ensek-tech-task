using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Infrastructure;

public static class ConfigureApplication
{
    public static void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IBulkUploadMeterReadings, BulkMeterReadingUploader>();
        serviceCollection.AddScoped<IParseMeterReadingsCsv, MeterReadingsCsvParser>();
    }
}

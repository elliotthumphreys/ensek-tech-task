using Microsoft.AspNetCore.Http;

namespace Application;

public interface IParseMeterReadingsCsv
{
    MeterReadingCsvProcessingResult ReadCsvFile(IFormFile meterReadingsCsv);
}

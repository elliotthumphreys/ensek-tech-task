using Microsoft.AspNetCore.Http;

namespace Application;

public interface IBulkUploadMeterReadings
{
    MeterReadingUploadResponseDto UploadMeterReadings(IFormFile meterReadingsCsv);
}

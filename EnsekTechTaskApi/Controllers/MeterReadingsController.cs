using Application;
using Microsoft.AspNetCore.Mvc;

namespace EnsekTechTaskApi.Controllers;

[ApiController]
public class MeterReadingsController : ControllerBase
{
    private readonly IBulkUploadMeterReadings _bulkUploadMeterReadings;

    public MeterReadingsController(IBulkUploadMeterReadings bulkUploadMeterReadings)
    {
        _bulkUploadMeterReadings = bulkUploadMeterReadings;
    }

    [HttpPost("meter-reading-uploads")]
    public IActionResult UploadMeterReadings(IFormFile meterReadingsCsv)
    {
        var response = _bulkUploadMeterReadings.UploadMeterReadings(meterReadingsCsv);

        return response switch
        {
            { Failed: > 0, Successfull: > 0 } => StatusCode(StatusCodes.Status207MultiStatus, response),
            { Failed: > 0 } => BadRequest(response),
            _ => Ok(response)
        };
    }
}

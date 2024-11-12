using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Application;

public class MeterReadingsCsvParser : IParseMeterReadingsCsv
{
    public MeterReadingCsvProcessingResult ReadCsvFile(IFormFile meterReadingsCsv)
    {
        if (meterReadingsCsv == null
            || !meterReadingsCsv.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)
            || meterReadingsCsv.ContentType != "text/csv")
            return new MeterReadingCsvProcessingResult(false, "Invalid file type provided - expecting file of type `text/csv` with extension `.csv`");

        if(meterReadingsCsv.Length == 0)
            return new MeterReadingCsvProcessingResult(false, "File had no content");

        using var reader = new StreamReader(meterReadingsCsv.OpenReadStream());

        var readings = ReadCsvFile(reader);

        return new MeterReadingCsvProcessingResult(true, null, readings);
    }

    private ICollection<MeterReadingInputDto> ReadCsvFile(StreamReader reader)
    {
        var readings = new List<MeterReadingInputDto>();

        int lineNumber = 0;
        while (!reader.EndOfStream)
        {
            lineNumber++;

            var line = reader.ReadLine();

            if (line == null)
                continue;

            if (line.StartsWith("accountId", StringComparison.OrdinalIgnoreCase))
                continue;

            readings.Add(ParseLine(line, lineNumber));
        }

        return readings;
    }

    private MeterReadingInputDto ParseLine(string line, int lineNumber)
    {
        var items = line.Split(',');

        if (items.Length < 3)
            return new MeterReadingInputDto(line, lineNumber);

        var canParseAccountId = int.TryParse(items[0].Trim(), out var accountId);
        var canParseMeterReaderValue = int.TryParse(items[2].Trim(), out var meterReaderValue);

        // 22/04/2019 12:25
        var dateTimeFormat = "dd/MM/yyyy HH:mm";
        var canParseDateTime = DateTime.TryParseExact(items[1].Trim(),
                                                      dateTimeFormat,
                                                      new CultureInfo("en-GB"),
                                                      DateTimeStyles.AssumeLocal,
                                                      out var dateTime);
        if (!canParseAccountId || !canParseMeterReaderValue || !canParseDateTime)
            return new MeterReadingInputDto(line, lineNumber);

        return new MeterReadingInputDto(line, lineNumber, accountId, dateTime, meterReaderValue);
    }
}

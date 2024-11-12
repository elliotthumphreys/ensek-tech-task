using DataAccess.Repositories;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application;

public class BulkMeterReadingUploader : IBulkUploadMeterReadings
{
    private readonly IParseMeterReadingsCsv _meterReadingCsvParser;
    private readonly IDataAccessRepository<Account> _dataAccessRepo;
    private readonly IUnitOfWork _unitOfWork;

    public BulkMeterReadingUploader(IDataAccessRepository<Account> dataAccessRepo,
                                    IUnitOfWork unitOfWork,
                                    IParseMeterReadingsCsv meterReadingCsvParser)
    {
        _dataAccessRepo = dataAccessRepo;
        _unitOfWork = unitOfWork;
        _meterReadingCsvParser = meterReadingCsvParser;
    }

    public MeterReadingUploadResponseDto UploadMeterReadings(IFormFile meterReadingsCsv)
    {
        var parsingResult = _meterReadingCsvParser.ReadCsvFile(meterReadingsCsv);

        if (!parsingResult.IsParsableFile || parsingResult.Readings == null)
            return MapResponseDto(parsingResult);

        var validInputs = parsingResult.Readings.Where(x => x.AccountId != null).ToList();
        var invalidInputs = parsingResult.Readings.Except(validInputs).ToList();

        if (validInputs.Any() &&  invalidInputs.Any())
            return new MeterReadingUploadResponseDto();

        var accountIds = validInputs.Select(x => x.AccountId).ToList();

        var accountsWithLatestReadings = _dataAccessRepo.Get(x => accountIds.Contains(x.Id))
            .Select(account => new
            {
                Account = account,
                LatestMeterReading = account.MeterReadings
                                            .OrderByDescending(mr => mr.ReadingDateTime)
                                            .FirstOrDefault()
            })
            .ToList();

        var accounts = accountsWithLatestReadings.Select(x => x.Account).ToList();

        var uploadResults = new List<ProcessMeterReadingResultDto>();

        foreach (var group in validInputs.GroupBy(x => x.AccountId))
        {
            var result = ProcessMeterReadingGroup(group, accounts);

            uploadResults.Add(result);
        }

        if(uploadResults.Any(x => x.Success))
            _unitOfWork.SaveChanges();

        return MapResponseDto(invalidInputs, uploadResults);
    }

    private MeterReadingUploadResponseDto MapResponseDto(MeterReadingCsvProcessingResult result)
    {
        var response = new MeterReadingUploadResponseDto
        {
            Failed = 1,
            Successfull = 0,
            FailureDetails = new List<string>
            {
                result.ParsingFailureReason ?? "Enexpected error when attempting to process csv file"
            }
        };

        return response;
    }

    private MeterReadingUploadResponseDto MapResponseDto(ICollection<MeterReadingInputDto> invalidInputs, ICollection<ProcessMeterReadingResultDto> uploadResults)
    {
        var failures = invalidInputs.Select(x => $"Unable to parse row {x.LineNumber}: `{x.OriginalRowString}`")
                                    .Union(uploadResults.Where(x => !x.Success).Select(x => $"Unable to persist {x.Input.LineNumber} due to: {x.FailureReason}"))
                                    .ToList();

        var response = new MeterReadingUploadResponseDto
        {
            Failed = failures.Count(),
            Successfull = uploadResults.Count(x => x.Success),
            FailureDetails = failures
        };

        return response;
    }

    private ProcessMeterReadingResultDto ProcessMeterReadingGroup(IGrouping<int?, MeterReadingInputDto> group, ICollection<Account> accounts)
    {
        var row = group.First();

        if (group.Count() > 1)
            return new ProcessMeterReadingResultDto(group.First(), false, $"Account id `{row.AccountId}` has been used on multiple rows, but should only appear once");

        var account = accounts.SingleOrDefault(x => x.Id == row.AccountId);
        if (account == null)
            return new ProcessMeterReadingResultDto(row, false, $"Account id `{row.AccountId}` does not exist");

        var result = account.AddMeterReading(row.MeterReadValue!.Value, row.MeterReadingDateTime!.Value);
        if (result.Success)
            return new ProcessMeterReadingResultDto(row, true);
        else
            return new ProcessMeterReadingResultDto(row, false, $"Account id `{group.First().AccountId}` failed to record reading due to: {result.Reason}");
    }

    public record struct ProcessMeterReadingResultDto(MeterReadingInputDto Input, bool Success, string? FailureReason = null);
}
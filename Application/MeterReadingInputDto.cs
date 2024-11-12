namespace Application;

public record struct MeterReadingCsvProcessingResult(bool IsParsableFile, 
                                                     string? ParsingFailureReason, 
                                                     IEnumerable<MeterReadingInputDto>? Readings = null);

public record struct MeterReadingInputDto(string OriginalRowString,
                                          int LineNumber,
                                          int? AccountId = null,
                                          DateTime? MeterReadingDateTime = null,
                                          int? MeterReadValue = null);

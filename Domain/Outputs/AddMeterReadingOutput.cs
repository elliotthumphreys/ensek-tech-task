namespace Domain.Outputs;

public record struct AddMeterReadingOutput(bool Success, string? Reason = null);

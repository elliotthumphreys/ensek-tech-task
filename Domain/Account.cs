using Domain.Outputs;

namespace Domain;

public class Account
{
    private Account()
    {
        FirstName = default!;
        LastName = default!;
        Version = default!;
        MeterReadings = new List<MeterReading>();
    }

    public Account(int id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Version = Guid.NewGuid();
        MeterReadings = new List<MeterReading>();
    }

    public int Id { get; private init; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    // used for concurrency control
    public Guid Version { get; private set; }

    public ICollection<MeterReading> MeterReadings { get; private set; }

    public AddMeterReadingOutput AddMeterReading(int readingValue, DateTime readingDateTime)
    {
        var utcDateTime = readingDateTime.ToUniversalTime();

        var latestedMeterReading = MeterReadings.MaxBy(x => x.ReadingDateTime);

        if (latestedMeterReading != null)
        {
            if (latestedMeterReading.ReadingValue == readingValue && latestedMeterReading.ReadingDateTime == utcDateTime)
                return new AddMeterReadingOutput(false, "Attempting to store duplicate reading");

            if (latestedMeterReading.ReadingDateTime >= utcDateTime)
                return new AddMeterReadingOutput(false, "Reading date time should be greater than the latest value we have on record");

            if (latestedMeterReading.ReadingValue > readingValue)
                return new AddMeterReadingOutput(false, "Reading can not be less than previous");
        }

        if (readingValue < 0)
            return new AddMeterReadingOutput(false, "Reading can not be less than zero");

        if (readingValue > 99999)
            return new AddMeterReadingOutput(false, "Reading can not be greater than 99999");

        Version = Guid.NewGuid();
        MeterReadings.Add(new(this, readingValue, utcDateTime));

        return new AddMeterReadingOutput(true);
    }
}
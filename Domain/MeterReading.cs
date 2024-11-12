namespace Domain;

public class MeterReading
{
    private MeterReading()
    {
        AssociatedAccount = default!;
    }

    internal MeterReading(Account account, int readingValue, DateTime readingDateTime)
    {
        AssociatedAccount = account;
        AssociatedAccountId = account.Id;

        ReadingValue = readingValue;
        ReadingDateTime = readingDateTime;
    }

    public int ReadingValue { get; private set; }
    public DateTime ReadingDateTime { get; private set; }
    public Account AssociatedAccount { get; private init; }
    public int AssociatedAccountId { get; private init; }
}

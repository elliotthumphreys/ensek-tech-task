namespace Application;

public class MeterReadingUploadResponseDto 
{
    public int Successfull { get; set; }
    public int Failed { get; set; }
    public ICollection<string> FailureDetails { get; set; }
}

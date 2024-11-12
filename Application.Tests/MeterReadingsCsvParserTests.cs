using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Text;
using Xunit;

namespace Application.Tests;

public class MeterReadingsCsvParserTests
{
    private readonly TestApplicationServiceProvider _applicationServiceProvider;

    public MeterReadingsCsvParserTests()
    {
        _applicationServiceProvider = new TestApplicationServiceProvider();
    }

    [Fact]
    public void ReadCsvFile_ReturnsError_WhenIncorrectNullFileProvided()
    {
        // arrange
        var csvParser = _applicationServiceProvider.GetSut<IParseMeterReadingsCsv>();

        // act
        var result = csvParser.ReadCsvFile(null);

        // assert
        Assert.False(result.IsParsableFile);
    }

    [Theory]
    [InlineData("testFile.csv", "json", 1)]
    [InlineData("testFile.json", "text/csv", 1)]
    [InlineData("testFile.csv", "text/csv", 0)]
    public void ReadCsvFile_ReturnsError_WhenIncorrectFileDataProvided(string? fileName, string? contentType, int contentLength)
    {
        // arrange
        var csvParser = _applicationServiceProvider.GetSut<IParseMeterReadingsCsv>();
        var substituteFile = Substitute.For<IFormFile>();

        substituteFile.OpenReadStream().Returns(new MemoryStream());
        substituteFile.FileName.Returns(fileName);
        substituteFile.ContentType.Returns(contentType);
        substituteFile.Length.Returns(contentLength);

        // act
        var result = csvParser.ReadCsvFile(substituteFile);

        // assert
        Assert.False(result.IsParsableFile);
    }

    [Fact]
    public void ReadCsvFile_ReturnsSuccess_WhenValidFileProvided()
    {
        // arrange
        var csvParser = _applicationServiceProvider.GetSut<IParseMeterReadingsCsv>();
        var substituteFile = Substitute.For<IFormFile>();

        var multiLineInput = "First line\nSecond line\nThird line";
        var byteArray = Encoding.UTF8.GetBytes(multiLineInput);
        using var fileContent = new MemoryStream(byteArray);

        substituteFile.FileName.Returns("testFile.csv");
        substituteFile.ContentType.Returns("text/csv");
        substituteFile.Length.Returns(byteArray.Length);
        substituteFile.OpenReadStream().Returns(fileContent);

        // act
        var result = csvParser.ReadCsvFile(substituteFile);

        // assert
        Assert.True(result.IsParsableFile);
    }

    [Theory]
    [InlineData(123, "22/04/2019 12:25", 0)]
    [InlineData(123, "22/04/2012 00:00", 1)]
    [InlineData(123, "01/01/2010 12:25", 99999)]
    public void ReadCsvFile_ReturnsSuccess_WhenValidReadingProvided(int accountId, string dateTime, int meterReadingValue)
    {
        // arrange
        var csvParser = _applicationServiceProvider.GetSut<IParseMeterReadingsCsv>();
        var substituteFile = Substitute.For<IFormFile>();

        var multiLineInput = new StringBuilder();

        multiLineInput.AppendLine("accountId, dateTime, meterreadingValue");
        multiLineInput.AppendLine($"{accountId},{dateTime}, {meterReadingValue}");

        var byteArray = Encoding.UTF8.GetBytes(multiLineInput.ToString());
        using var fileContent = new MemoryStream(byteArray);

        substituteFile.FileName.Returns("testFile.csv");
        substituteFile.ContentType.Returns("text/csv");
        substituteFile.Length.Returns(byteArray.Length);
        substituteFile.OpenReadStream().Returns(fileContent);

        // act
        var result = csvParser.ReadCsvFile(substituteFile);

        // assert
        Assert.True(result.IsParsableFile);
        Assert.NotNull(result.Readings);
        Assert.Single(result.Readings);

        var reading = result.Readings.Single();

        Assert.Equal(2, reading.LineNumber);
        Assert.Equal(accountId, reading.AccountId);
        Assert.Equal(meterReadingValue, reading.MeterReadValue);
    }

    [Theory]
    [InlineData("invalidAccountId, 22/04/2019 12:25, 0")]
    [InlineData("123, invalidDateTime, 1")]
    [InlineData("123, 01/01/2010 12:25, invalidReadingValue")]
    public void ReadCsvFile_ReturnsSuccess_WhenInValidReadingProvided(string row)
    {
        // arrange
        var csvParser = _applicationServiceProvider.GetSut<IParseMeterReadingsCsv>();
        var substituteFile = Substitute.For<IFormFile>();

        var multiLineInput = new StringBuilder();

        multiLineInput.AppendLine("accountId, dateTime, meterreadingValue");
        multiLineInput.AppendLine(row);

        var byteArray = Encoding.UTF8.GetBytes(multiLineInput.ToString());
        using var fileContent = new MemoryStream(byteArray);

        substituteFile.FileName.Returns("testFile.csv");
        substituteFile.ContentType.Returns("text/csv");
        substituteFile.Length.Returns(byteArray.Length);
        substituteFile.OpenReadStream().Returns(fileContent);

        // act
        var result = csvParser.ReadCsvFile(substituteFile);

        // assert
        Assert.True(result.IsParsableFile);
        Assert.NotNull(result.Readings);
        Assert.Single(result.Readings);

        var reading = result.Readings.Single();

        Assert.Equal(2, reading.LineNumber);
        Assert.Equal(row, reading.OriginalRowString);
        
        Assert.Null(reading.AccountId);
        Assert.Null(reading.MeterReadingDateTime);
        Assert.Null(reading.MeterReadValue);
    }

}


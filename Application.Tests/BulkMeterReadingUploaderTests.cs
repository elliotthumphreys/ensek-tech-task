using Microsoft.AspNetCore.Http;
using System.Text;
using Xunit;
using NSubstitute;
using Domain;
using Microsoft.Extensions.DependencyInjection;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Tests;

public class BulkMeterReadingUploaderTests
{
    [Fact]
    public void UploadMeterReadings_ReturnFailure_WhenParsingFileFails()
    {
        // arrange
        var uploader = new TestApplicationServiceProvider().GetSut<IBulkUploadMeterReadings>();

        // act
        var result = uploader.UploadMeterReadings(null);

        // assert
        Assert.Equal(1, result.Failed);
        Assert.Equal(0, result.Successfull);
    }

    [Fact]
    public void UploadMeterReadings_ReturnFailure_WhenAccountDoesNotExist()
    {
        // arrange
        var uploader = new TestApplicationServiceProvider().GetSut<IBulkUploadMeterReadings>();
        var (file, stream) = GetIFormFileSubstitute("-1, 01/01/2010 12:25, 0");

        // act
        var result = uploader.UploadMeterReadings(file);

        // assert
        Assert.Equal(1, result.Failed);
        Assert.Equal(0, result.Successfull);

        // cleanup
        stream.Dispose();
    }

    [Theory]
    [InlineData("01/31/2010 12:25")]
    [InlineData("01/01/2010 12:25:000T00")]
    [InlineData("01-01-2010 12:25")]
    [InlineData("20130623T13:22-0500")]
    public void UploadMeterReadings_ReturnFailure_WhenDateTimeInWrongFormat(string dateTime)
    {
        // arrange
        var uploader = new TestApplicationServiceProvider().GetSut<IBulkUploadMeterReadings>();
        var (file, stream) = GetIFormFileSubstitute($"-1, {dateTime}, 0");

        // act
        var result = uploader.UploadMeterReadings(file);

        // assert
        Assert.Equal(1, result.Failed);
        Assert.Equal(0, result.Successfull);

        // cleanup
        stream.Dispose();
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("100000")]
    public void UploadMeterReadings_ReturnFailure_WhenReadingIsInvalid(string reading)
    {
        // arrange
        var uploader = new TestApplicationServiceProvider().GetSut<IBulkUploadMeterReadings>();
        var (file, stream) = GetIFormFileSubstitute($"-1, 01/01/2010 12:25, {reading}");

        // act
        var result = uploader.UploadMeterReadings(file);

        // assert
        Assert.Equal(1, result.Failed);
        Assert.Equal(0, result.Successfull);

        // cleanup
        stream.Dispose();
    }

    [Fact]
    public void UploadMeterReadings_ReturnFailure_WhenReadingOlderThanLatest()
    {
        // arrange
        var testServiceProvider = new TestApplicationServiceProvider();

        var accountId = 9999;
        var existingAccount = new Account(accountId, "exampleName", "exampleLastName");
        existingAccount.AddMeterReading(1, DateTime.UtcNow);

        var accountsRepo = testServiceProvider.ServiceProvider.GetRequiredService<IDataAccessRepository<Account>>();
        var unitOfWork = testServiceProvider.ServiceProvider.GetRequiredService<IUnitOfWork>();

        accountsRepo.Add(existingAccount);
        unitOfWork.SaveChanges();

        var uploader = testServiceProvider.GetSut<IBulkUploadMeterReadings>();

        var (file, stream) = GetIFormFileSubstitute($"{accountId}, 01/01/2010 12:25, 2");

        // act
        var result = uploader.UploadMeterReadings(file);

        // assert
        Assert.Equal(1, result.Failed);
        Assert.Equal(0, result.Successfull);

        // cleanup
        stream.Dispose();
    }

    [Fact]
    public void UploadMeterReadings_ReturnFailure_WhenReadingValueLessThanLatest()
    {
        // arrange
        var testServiceProvider = new TestApplicationServiceProvider();

        var accountId = 9999;
        var existingAccount = new Account(accountId, "exampleName", "exampleLastName");
        existingAccount.AddMeterReading(100, DateTime.UtcNow);

        var accountsRepo = testServiceProvider.ServiceProvider.GetRequiredService<IDataAccessRepository<Account>>();
        var unitOfWork = testServiceProvider.ServiceProvider.GetRequiredService<IUnitOfWork>();

        accountsRepo.Add(existingAccount);
        unitOfWork.SaveChanges();

        var uploader = testServiceProvider.GetSut<IBulkUploadMeterReadings>();

        var (file, stream) = GetIFormFileSubstitute($"{accountId}, 01/01/2010 12:25, 0");

        // act
        var result = uploader.UploadMeterReadings(file);

        // assert
        Assert.Equal(1, result.Failed);
        Assert.Equal(0, result.Successfull);

        // cleanup
        stream.Dispose();
    }

    [Fact]
    public void UploadMeterReadings_ReturnSuccess_AndEnsureReadingPersisted_WhenNewReadingAdded()
    {
        // arrange
        var testServiceProvider = new TestApplicationServiceProvider();

        var accountId = 9999;
        var existingAccount = new Account(accountId, "exampleName", "exampleLastName");

        var accountsRepo = testServiceProvider.ServiceProvider.GetRequiredService<IDataAccessRepository<Account>>();
        var unitOfWork = testServiceProvider.ServiceProvider.GetRequiredService<IUnitOfWork>();

        accountsRepo.Add(existingAccount);
        unitOfWork.SaveChanges();

        var uploader = testServiceProvider.GetSut<IBulkUploadMeterReadings>();

        var readingValue = 89;
        var readingDay = "01";
        var readingMonth = "02";
        var readingYeah = 2010;
        var readingHour = 12;
        var readingMinute = 25;

        var (file, stream) = GetIFormFileSubstitute($"{accountId}, {readingDay}/{readingMonth}/{readingYeah} {readingHour}:{readingMinute}, {readingValue}");

        // act
        var result = uploader.UploadMeterReadings(file);

        // assert
        Assert.Equal(0, result.Failed);
        Assert.Equal(1, result.Successfull);

        var updatedAccount = accountsRepo.Get(x => x.Id == accountId)
                                         .Include(x => x.MeterReadings)
                                         .Single();

        Assert.NotEmpty(updatedAccount.MeterReadings);
        Assert.Single(updatedAccount.MeterReadings);

        var persistedReading = updatedAccount.MeterReadings.Single();

        Assert.Equal(readingValue, persistedReading.ReadingValue);
        Assert.Equal(int.Parse(readingDay), persistedReading.ReadingDateTime.Day);
        Assert.Equal(int.Parse(readingMonth), persistedReading.ReadingDateTime.Month);
        Assert.Equal(readingYeah, persistedReading.ReadingDateTime.Year);
        Assert.Equal(readingHour, persistedReading.ReadingDateTime.Hour);
        Assert.Equal(readingMinute, persistedReading.ReadingDateTime.Minute);

        // cleanup
        stream.Dispose();
    }

    private (IFormFile, MemoryStream) GetIFormFileSubstitute(params string[] rows)
    {
        var substituteFile = Substitute.For<IFormFile>();

        var multiLineInput = new StringBuilder();

        multiLineInput.AppendLine("accountId, dateTime, meterreadingValue");

        foreach(var row in rows)
            multiLineInput.AppendLine(row);

        var byteArray = Encoding.UTF8.GetBytes(multiLineInput.ToString());
        var fileContent = new MemoryStream(byteArray);

        substituteFile.FileName.Returns("testFile.csv");
        substituteFile.ContentType.Returns("text/csv");
        substituteFile.Length.Returns(byteArray.Length);
        substituteFile.OpenReadStream().Returns(fileContent);

        return (substituteFile, fileContent);
    }
}
using Moq;
using ClosedXML.Excel;
using DataManager.Core.Services;
using DataManager.Core;
using DataManager.Core.DBModels;

namespace DataManager.Test;

public class DataModelTwoServiceTests
{
    private enum TestDataScenario
    {
        ValidData,
        InvalidDate,
        InvalidGainAmountThree
    }

    private static void CreateTestXlsxData(string filePath, TestDataScenario scenario)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Sheet1");
        worksheet.Cell(1, 2).Value = "Report"; // Needed because ReadXlsx skips the first two non-empty rows
        worksheet.Cell(3, 1).Value = "PeriodStartDate";
        worksheet.Cell(3, 2).Value = "Exit";
        worksheet.Cell(3, 3).Value = "Gain Amount Three";
        worksheet.Cell(3, 4).Value = "PeriodEndDate";

        switch (scenario)
        {
            case TestDataScenario.ValidData:
                worksheet.Cell(4, 1).Value = "01.01.2024.";
                worksheet.Cell(4, 2).Value = "Exit1";
                worksheet.Cell(4, 3).Value = 100;
                worksheet.Cell(4, 4).Value = "01.01.2024. 12:00:00";

                worksheet.Cell(5, 1).Value = "02.01.2024.";
                worksheet.Cell(5, 2).Value = "Exit2";
                worksheet.Cell(5, 3).Value = 150;
                worksheet.Cell(5, 4).Value = "02.01.2024. 12:00:00";
                break;

            case TestDataScenario.InvalidDate:
                worksheet.Cell(4, 1).Value = "InvalidDate"; // Invalid data for PeriodStartDate
                worksheet.Cell(4, 2).Value = "Exit1";
                worksheet.Cell(4, 3).Value = 100;
                worksheet.Cell(4, 4).Value = "01.01.2024. 12:00:00";

                worksheet.Cell(5, 1).Value = "02.01.2024.";
                worksheet.Cell(5, 2).Value = "Exit2";
                worksheet.Cell(5, 3).Value = 150;
                worksheet.Cell(5, 4).Value = "02.01.2024. 12:00:00";
                break;

            case TestDataScenario.InvalidGainAmountThree:
                worksheet.Cell(4, 1).Value = "01.01.2024.";
                worksheet.Cell(4, 2).Value = "Exit1";
                worksheet.Cell(4, 3).Value = "InvalidData"; // Invalid data for Gain Amount Three
                worksheet.Cell(4, 4).Value = "01.01.2024. 12:00:00";

                worksheet.Cell(5, 1).Value = "02.01.2024.";
                worksheet.Cell(5, 2).Value = "Exit2";
                worksheet.Cell(5, 3).Value = 150;
                worksheet.Cell(5, 4).Value = "02.01.2024. 12:00:00";
                break;
        }

        workbook.SaveAs(filePath);
    }

    [Fact]
    public void ParseXlsx_DataParsedCorrectly_ReturnsDataModelTwoList()
    {
        // Arrange
        // File location: ...\DataManager.Test\bin\Debug\net8.0
        string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ValidDataModelTwoTestData.xlsx");
        CreateTestXlsxData(testFilePath, TestDataScenario.ValidData);

        // Act
        var dataModelTwos = DataModelTwoService.ParseXlsx(testFilePath);

        // Assert
        Assert.NotNull(dataModelTwos);
        Assert.NotEmpty(dataModelTwos);
        Assert.Equal(2, dataModelTwos.Count); // Check if the correct number of dataModelTwos are read

        // Check if the data of the first dataModelTwo is read correctly
        Assert.Equal(new DateOnly(2024, 01, 01), dataModelTwos[0].PeriodStartDate);
        Assert.Equal("Exit1", dataModelTwos[0].Exit.Name);
        Assert.Equal(100, dataModelTwos[0].GainAmountThree);
        Assert.Equal(new DateTime(2024, 01, 01, 12, 0, 0), dataModelTwos[0].PeriodEndDate);

        // Check if the data of the second dataModelTwo is read correctly
        Assert.Equal(new DateOnly(2024, 01, 02), dataModelTwos[1].PeriodStartDate);
        Assert.Equal("Exit2", dataModelTwos[1].Exit.Name);
        Assert.Equal(150, dataModelTwos[1].GainAmountThree);
        Assert.Equal(new DateTime(2024, 01, 02, 12, 0, 0), dataModelTwos[1].PeriodEndDate);

        // Clean up test file
        File.Delete(testFilePath);
    }

    [Fact]
    public void ParseXlsx_InvalidDateData_ReturnsEmptyList()
    {
        // Arrange
        string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "InvalidDateDataModelTwoTestData.xlsx");
        CreateTestXlsxData(testFilePath, TestDataScenario.InvalidDate);

        // Act
        var dataModelTwos = DataModelTwoService.ParseXlsx(testFilePath);

        // Assert
        Assert.NotNull(dataModelTwos);
        Assert.Empty(dataModelTwos); // Expecting an empty list due to invalid data

        // Clean up test file
        File.Delete(testFilePath);
    }

    [Fact]
    public void ParseXlsx_InvalidDataModelTwoData_ReturnsEmptyList()
    {
        // Arrange
        string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "InvalidDataModelTwoTestData.xlsx");
        CreateTestXlsxData(testFilePath, TestDataScenario.InvalidGainAmountThree);

        // Act
        var dataModelTwos = DataModelTwoService.ParseXlsx(testFilePath);

        // Assert
        Assert.NotNull(dataModelTwos);
        Assert.Empty(dataModelTwos); // Expecting an empty list due to invalid data

        // Clean up test file
        File.Delete(testFilePath);
    }

    [Fact]
    public void FetchDataModelTwoData_NoFilters_ReturnsAllDataModelTwosOrderedByDate()
    {
        // Arrange
        var contextMock = new Mock<DataManagerDbContext>();
        var dateFrom = new DateOnly(2024, 1, 1);
        var dateTo = new DateOnly(2024, 6, 1);
        var exitId = default(int); // No filter on exitId

        var dataModelTwoData = new List<DataModelTwo>
    {
        new() { PeriodStartDate = new DateOnly(2024, 1, 5), ExitId = 1 },
        new() { PeriodStartDate = new DateOnly(2024, 2, 10), ExitId = 2 },
    };

        // Mocking the dataModelTwos DbSet in the context
        var mockDataModelTwos = MockDbSetHelper.CreateMockDbSet(dataModelTwoData);
        contextMock.Setup(c => c.DataModelTwos).Returns(mockDataModelTwos.Object);

        // Act
        var result = DataModelTwoService.FetchDataModelTwoData(contextMock.Object, dateFrom, dateTo, exitId);

        // Assert
        // Verify that the result contains all dataModelTwos within the date range, ordered by date
        Assert.Equal(dataModelTwoData.OrderBy(f => f.PeriodStartDate), result);
    }

    [Fact]
    public void FetchDataModelTwoData_DateRangeFilter_ReturnsDataModelTwosWithinRangeOrderedByDate()
    {
        // Arrange
        var contextMock = new Mock<DataManagerDbContext>();
        var dateFrom = new DateOnly(2024, 1, 1);
        var dateTo = new DateOnly(2024, 6, 1);
        var exitId = default(int); // No filter on exitId

        var dataModelTwoData = new List<DataModelTwo>
    {
        new() { PeriodStartDate = new DateOnly(2024, 1, 5), ExitId = 1 },
        new() { PeriodStartDate = new DateOnly(2024, 2, 10), ExitId = 2 },
    };

        // Mocking the dataModelTwos DbSet in the context
        var mockDataModelTwos = MockDbSetHelper.CreateMockDbSet(dataModelTwoData);
        contextMock.Setup(c => c.DataModelTwos).Returns(mockDataModelTwos.Object);

        // Act
        var result = DataModelTwoService.FetchDataModelTwoData(contextMock.Object, dateFrom, dateTo, exitId);

        // Assert
        // Verify that the result contains all dataModelTwos within the date range, ordered by date
        Assert.Equal(dataModelTwoData.Where(f => f.PeriodStartDate >= dateFrom && f.PeriodStartDate <= dateTo).OrderBy(f => f.PeriodStartDate), result);
    }

    [Fact]
    public void FetchDataModelTwoData_ExitIdFilter_ReturnsDataModelTwosForSpecificExitOrderedByExitIdThenDate()
    {
        // Arrange
        var contextMock = new Mock<DataManagerDbContext>();
        var dateFrom = default(DateOnly);
        var dateTo = default(DateOnly);
        var exitId = 1; // Example exit ID

        var dataModelTwoData = new List<DataModelTwo>
    {
        new() { PeriodStartDate = new DateOnly(2024, 1, 5), ExitId = 1 },
        new() { PeriodStartDate = new DateOnly(2024, 2, 10), ExitId = 1 },
    };

        // Mocking the dataModelTwos DbSet in the context
        var mockDataModelTwos = MockDbSetHelper.CreateMockDbSet(dataModelTwoData);
        contextMock.Setup(c => c.DataModelTwos).Returns(mockDataModelTwos.Object);

        // Act
        var result = DataModelTwoService.FetchDataModelTwoData(contextMock.Object, dateFrom, dateTo, exitId);

        // Assert
        // Verify that the result contains dataModelTwos only for the specified exit ID, ordered by exit ID then date
        Assert.Equal(dataModelTwoData.Where(f => f.ExitId == exitId).OrderBy(f => f.ExitId).ThenBy(f => f.PeriodStartDate), result);
    }
}
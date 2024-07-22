using Moq;
using ClosedXML.Excel;
using DataManager.Core;
using DataManager.Core.Services;
using DataManager.Core.DBModels;

namespace DataManager.Test;

public class ModelTwoServiceTests
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
    public void ParseXlsx_DataParsedCorrectly_ReturnsModelTwoList()
    {
        // Arrange
        // File location: ...\DataManager.Test\bin\Debug\net8.0
        string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ValidModelTwoTestData.xlsx");
        CreateTestXlsxData(testFilePath, TestDataScenario.ValidData);

        // Act
        var modelTwos = ModelTwoService.ParseXlsx(testFilePath);

        // Assert
        Assert.NotNull(modelTwos);
        Assert.NotEmpty(modelTwos);
        Assert.Equal(2, modelTwos.Count); // Check if the correct number of ModelTwos are read

        // Check if the data of the first ModelTwo is read correctly
        Assert.Equal(new DateOnly(2024, 01, 01), modelTwos[0].PeriodStartDate);
        Assert.Equal("Exit1", modelTwos[0].Exit.Name);
        Assert.Equal(100, modelTwos[0].GainAmountThree);
        Assert.Equal(new DateTime(2024, 01, 01, 12, 0, 0), modelTwos[0].PeriodEndDate);

        // Check if the data of the second ModelTwo is read correctly
        Assert.Equal(new DateOnly(2024, 01, 02), modelTwos[1].PeriodStartDate);
        Assert.Equal("Exit2", modelTwos[1].Exit.Name);
        Assert.Equal(150, modelTwos[1].GainAmountThree);
        Assert.Equal(new DateTime(2024, 01, 02, 12, 0, 0), modelTwos[1].PeriodEndDate);

        // Clean up test file
        File.Delete(testFilePath);
    }

    [Fact]
    public void ParseXlsx_InvalidDateData_ReturnsEmptyList()
    {
        // Arrange
        string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "InvalidModelTwoTestData.xlsx");
        CreateTestXlsxData(testFilePath, TestDataScenario.InvalidDate);

        // Act
        var modelTwos = ModelTwoService.ParseXlsx(testFilePath);

        // Assert
        Assert.NotNull(modelTwos);
        Assert.Empty(modelTwos); // Expecting an empty list due to invalid data

        // Clean up test file
        File.Delete(testFilePath);
    }

    [Fact]
    public void ParseXlsx_InvalidDataModelTwoData_ReturnsEmptyList()
    {
        // Arrange
        string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "InvalidModelTwoTestData.xlsx");
        CreateTestXlsxData(testFilePath, TestDataScenario.InvalidGainAmountThree);

        // Act
        var modelTwos = ModelTwoService.ParseXlsx(testFilePath);

        // Assert
        Assert.NotNull(modelTwos);
        Assert.Empty(modelTwos); // Expecting an empty list due to invalid data

        // Clean up test file
        File.Delete(testFilePath);
    }

    [Fact]
    public void FetchModelTwoData_NoFilters_ReturnsAllModelTwosOrderedByDate()
    {
        // Arrange
        var contextMock = new Mock<DataManagerDbContext>();
        var dateFrom = new DateOnly(2024, 1, 1);
        var dateTo = new DateOnly(2024, 6, 1);
        var exitId = default(int); // No filter on exitId

        var modelTwoData = new List<ModelTwo>
    {
        new() { PeriodStartDate = new DateOnly(2024, 1, 5), ExitId = 1 },
        new() { PeriodStartDate = new DateOnly(2024, 2, 10), ExitId = 2 },
    };

        // Mocking the ModelTwos DbSet in the context
        var mockDataModelTwos = MockDbSetHelper.CreateMockDbSet(modelTwoData);
        contextMock.Setup(c => c.ModelTwos).Returns(mockDataModelTwos.Object);

        // Act
        var result = ModelTwoService.FetchModelTwoData(contextMock.Object, dateFrom, dateTo, exitId);

        // Assert
        // Verify that the result contains all ModelTwos within the date range, ordered by date
        Assert.Equal(modelTwoData.OrderBy(f => f.PeriodStartDate), result);
    }

    [Fact]
    public void FetchModelTwoData_DateRangeFilter_ReturnsModelTwosWithinRangeOrderedByDate()
    {
        // Arrange
        var contextMock = new Mock<DataManagerDbContext>();
        var dateFrom = new DateOnly(2024, 1, 1);
        var dateTo = new DateOnly(2024, 6, 1);
        var exitId = default(int); // No filter on exitId

        var modelTwoData = new List<ModelTwo>
    {
        new() { PeriodStartDate = new DateOnly(2024, 1, 5), ExitId = 1 },
        new() { PeriodStartDate = new DateOnly(2024, 2, 10), ExitId = 2 },
    };

        // Mocking the ModelTwos DbSet in the context
        var mockDataModelTwos = MockDbSetHelper.CreateMockDbSet(modelTwoData);
        contextMock.Setup(c => c.ModelTwos).Returns(mockDataModelTwos.Object);

        // Act
        var result = ModelTwoService.FetchModelTwoData(contextMock.Object, dateFrom, dateTo, exitId);

        // Assert
        // Verify that the result contains all ModelTwos within the date range, ordered by date
        Assert.Equal(modelTwoData.Where(f => f.PeriodStartDate >= dateFrom && f.PeriodStartDate <= dateTo).OrderBy(f => f.PeriodStartDate), result);
    }

    [Fact]
    public void FetchModelTwoData_ExitIdFilter_ReturnsModelTwosForSpecificExitOrderedByExitIdThenDate()
    {
        // Arrange
        var contextMock = new Mock<DataManagerDbContext>();
        var dateFrom = default(DateOnly);
        var dateTo = default(DateOnly);
        var exitId = 1; // Example exit ID

        var modelTwoData = new List<ModelTwo>
    {
        new() { PeriodStartDate = new DateOnly(2024, 1, 5), ExitId = 1 },
        new() { PeriodStartDate = new DateOnly(2024, 2, 10), ExitId = 1 },
    };

        // Mocking the ModelTwos DbSet in the context
        var mockDataModelTwos = MockDbSetHelper.CreateMockDbSet(modelTwoData);
        contextMock.Setup(c => c.ModelTwos).Returns(mockDataModelTwos.Object);

        // Act
        var result = ModelTwoService.FetchModelTwoData(contextMock.Object, dateFrom, dateTo, exitId);

        // Assert
        // Verify that the result contains ModelTwos only for the specified exit ID, ordered by exit ID then date
        Assert.Equal(modelTwoData.Where(f => f.ExitId == exitId).OrderBy(f => f.ExitId).ThenBy(f => f.PeriodStartDate), result);
    }
}
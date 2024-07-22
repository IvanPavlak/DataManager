using DataManager.Core;
using DataManager.Core.Services;
using DataManager.Core.DBModels;
using Moq;

namespace DataManager.Test;

public class CombinedDataServiceTests
{
    [Fact]
    public void FetchCombinedData_ReturnsCombinedDataList()
    {
        // Arrange
        var dateFrom = new DateOnly(2023, 1, 1);
        var dateTo = new DateOnly(2023, 12, 31);
        var exitId = 1;

        var modelOneData = new List<ModelOne>
            {
                new() { Date = new DateOnly(2023, 6, 1), ExitId = 1, Total = 100 },
                new() { Date = new DateOnly(2023, 6, 2), ExitId = 1, Total = 200 }
            };

        var modelTwoData = new List<ModelTwo>
            {
                new() { PeriodStartDate = new DateOnly(2023, 6, 1), ExitId = 1, GainAmountThree = 150 },
                new() { PeriodStartDate = new DateOnly(2023, 6, 2), ExitId = 1, GainAmountThree = 250 }
            };

        var exits = new List<Exit>
            {
                new() { Id = 1, Name = "Exit 1" }
            };

        var mockContext = new Mock<DataManagerDbContext>();

        var mockModelOnesDbSet = MockDbSetHelper.CreateMockDbSet(modelOneData);
        var mockModelTwosDbSet = MockDbSetHelper.CreateMockDbSet(modelTwoData);
        var mockExitsDbSet = MockDbSetHelper.CreateMockDbSet(exits);

        mockContext.Setup(c => c.ModelOnes).Returns(mockModelOnesDbSet.Object);
        mockContext.Setup(c => c.ModelTwos).Returns(mockModelTwosDbSet.Object);
        mockContext.Setup(c => c.Exits).Returns(mockExitsDbSet.Object);

        // Act
        var result = CombinedDataService.FetchCombinedData(mockContext.Object, dateFrom, dateTo, exitId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(new DateOnly(2023, 6, 1), result[0].Date);
        Assert.Equal(1, result[0].ExitId);
        Assert.Equal("Exit 1", result[0].ExitName);
        Assert.Equal(100, result[0].TotalModelOne);
        Assert.Equal(150, result[0].TotalModelTwo);
        Assert.Equal(66.67, result[0].Ratio, 2); // Using tolerance for floating point comparison
    }

    [Fact]
    public void FetchCombinedData_NoMatchingModelTwoData_ReturnsEmptyList()
    {
        // Arrange
        var dateFrom = new DateOnly(2023, 1, 1);
        var dateTo = new DateOnly(2023, 12, 31);
        var exitId = 1;

        var modelTwoData = new List<ModelTwo>(); // No matching ModelTwo data

        var modelOneData = new List<ModelOne>
        {
            new() { Date = new DateOnly(2023, 6, 1), ExitId = 1, Total = 100 }
        };

        var exits = new List<Exit>
        {
            new() { Id = 1, Name = "Exit 1" }
        };

        var mockContext = new Mock<DataManagerDbContext>();

        var mockModelOnesDbSet = MockDbSetHelper.CreateMockDbSet(modelOneData);
        var mockModelTwosDbSet = MockDbSetHelper.CreateMockDbSet(modelTwoData);
        var mockExitsDbSet = MockDbSetHelper.CreateMockDbSet(exits);

        mockContext.Setup(c => c.ModelOnes).Returns(mockModelOnesDbSet.Object);
        mockContext.Setup(c => c.ModelTwos).Returns(mockModelTwosDbSet.Object);
        mockContext.Setup(c => c.Exits).Returns(mockExitsDbSet.Object);

        // Act
        var result = CombinedDataService.FetchCombinedData(mockContext.Object, dateFrom, dateTo, exitId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FetchCombinedData_NoMatchingModelOneData_ReturnsEmptyList()
    {
        // Arrange
        var dateFrom = new DateOnly(2023, 1, 1);
        var dateTo = new DateOnly(2023, 12, 31);
        var exitId = 1;

        var modelOneData = new List<ModelOne>(); // No matching ModelOne data

        var modelTwoData = new List<ModelTwo>
        {
            new() { PeriodStartDate = new DateOnly(2023, 6, 1), ExitId = 1, GainAmountThree = 150 }
        };
        var exits = new List<Exit>
        {
            new() { Id = 1, Name = "Exit 1" }
        };

        var mockContext = new Mock<DataManagerDbContext>();

        var mockModelOnesDbSet = MockDbSetHelper.CreateMockDbSet(modelOneData);
        var mockModelTwosDbSet = MockDbSetHelper.CreateMockDbSet(modelTwoData);
        var mockExitsDbSet = MockDbSetHelper.CreateMockDbSet(exits);

        mockContext.Setup(c => c.ModelOnes).Returns(mockModelOnesDbSet.Object);
        mockContext.Setup(c => c.ModelTwos).Returns(mockModelTwosDbSet.Object);
        mockContext.Setup(c => c.Exits).Returns(mockExitsDbSet.Object);

        // Act
        var result = CombinedDataService.FetchCombinedData(mockContext.Object, dateFrom, dateTo, exitId);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FetchCombinedData_NoMatchingName_ReturnsUnknownName()
    {
        // Arrange
        var dateFrom = new DateOnly(2023, 1, 1);
        var dateTo = new DateOnly(2023, 12, 31);
        var exitId = 1;

        var exits = new List<Exit>(); // No matching exit name

        var modelOneData = new List<ModelOne>
            {
                new() { Date = new DateOnly(2023, 6, 1), ExitId = 1, Total = 100 }
            };

        var modelTwoData = new List<ModelTwo>
            {
                new() { PeriodStartDate = new DateOnly(2023, 6, 1), ExitId = 1, GainAmountThree = 150 }
            };

        var mockContext = new Mock<DataManagerDbContext>();

        var mockModelOnesDbSet = MockDbSetHelper.CreateMockDbSet(modelOneData);
        var mockModelTwosDbSet = MockDbSetHelper.CreateMockDbSet(modelTwoData);
        var mockExitsDbSet = MockDbSetHelper.CreateMockDbSet(exits);

        mockContext.Setup(c => c.ModelOnes).Returns(mockModelOnesDbSet.Object);
        mockContext.Setup(c => c.ModelTwos).Returns(mockModelTwosDbSet.Object);
        mockContext.Setup(c => c.Exits).Returns(mockExitsDbSet.Object);

        // Act
        var result = CombinedDataService.FetchCombinedData(mockContext.Object, dateFrom, dateTo, exitId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(new DateOnly(2023, 6, 1), result[0].Date);
        Assert.Equal(1, result[0].ExitId);
        Assert.Equal("Unknown", result[0].ExitName);
        Assert.Equal(100, result[0].TotalModelOne);
        Assert.Equal(150, result[0].TotalModelTwo);
        Assert.Equal(66.67, result[0].Ratio, 2);
    }

    [Fact]
    public void FetchCombinedData_EmptyData_ReturnsEmptyList()
    {
        // Arrange
        var dateFrom = new DateOnly(2023, 1, 1);
        var dateTo = new DateOnly(2023, 12, 31);
        var exitId = 1;

        var modelOneData = new List<ModelOne>(); // Empty data
        var modelTwoData = new List<ModelTwo>(); // Empty data
        var exits = new List<Exit>(); // Empty data

        var mockContext = new Mock<DataManagerDbContext>();

        var mockModelOnesDbSet = MockDbSetHelper.CreateMockDbSet(modelOneData);
        var mockModelTwosDbSet = MockDbSetHelper.CreateMockDbSet(modelTwoData);
        var mockExitsDbSet = MockDbSetHelper.CreateMockDbSet(exits);

        mockContext.Setup(c => c.ModelOnes).Returns(mockModelOnesDbSet.Object);
        mockContext.Setup(c => c.ModelTwos).Returns(mockModelTwosDbSet.Object);
        mockContext.Setup(c => c.Exits).Returns(mockExitsDbSet.Object);

        // Act
        var result = CombinedDataService.FetchCombinedData(mockContext.Object, dateFrom, dateTo, exitId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
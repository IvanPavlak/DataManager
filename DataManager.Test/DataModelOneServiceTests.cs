using Moq;
using DataManager.Core.Services;
using DataManager.Core;
using DataManager.Core.DBModels;

namespace DataManager.Test
{
    public class DataModelOneServiceTests
    {
        private enum TestDataScenario
        {
            ValidData,
            InvalidDate,
            InvalidGainAmountOne
        }

        private static void CreateTestCsvData(string filePath, TestDataScenario scenario)
        {
            switch (scenario)
            {
                case TestDataScenario.ValidData:
                    File.WriteAllText(filePath, "Exit;Port;User Group;Country;Member ID;Date;Gain Amount One;Gain Amount Two;Loss Amount;Total Amount\n"
                                    + "Exit1;Port1;UserGroup1;Country1;1;2024-01-01;5000;3000;500;7500\n"
                                    + "Exit2;Port2;UserGroup2;Country2;2;2024-01-02;6000;4000;600;9400\n");
                    break;
                case TestDataScenario.InvalidDate:
                    File.WriteAllText(filePath, "Exit;Port;User Group;Country;Member ID;Date;Gain Amount One;Gain Amount Two;Loss Amount;Total Amount\n"
                                    + "Exit1;Port1;UserGroup1;Country1;1;InvalidDate;5000;3000;500;7500\n" // Invalid data for Date
                                    + "Exit2;Port2;UserGroup2;Country2;2;2024-01-02;6000;4000;600;9400\n");
                    break;
                case TestDataScenario.InvalidGainAmountOne:
                    File.WriteAllText(filePath, "Exit;Port;User Group;Country;Member ID;Date;Gain Amount One;Gain Amount Two;Loss Amount;Total Amount\n"
                                    + "Exit1;Port1;UserGroup1;Country1;1;2024-01-01;InvalidData;3000;500;7500\n" // Invalid data for Gain Amount One
                                    + "Exit2;Port2;UserGroup2;Country2;2;2024-01-02;6000;4000;600;9400\n");
                    break;
            }
        }

        [Fact]
        public void ParseCsv_DataParsedCorrectly_ReturnsDataModelOneList()
        {
            // Arrange
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ValidDataModelOneTestData.csv");
            CreateTestCsvData(testFilePath, TestDataScenario.ValidData);

            // Act
            var dataModelOnes = DataModelOneService.ParseCsv(testFilePath);

            // Assert
            Assert.NotNull(dataModelOnes);
            Assert.NotEmpty(dataModelOnes);
            Assert.Equal(2, dataModelOnes.Count); // Check if the correct number of dataModelOnes are read

            // Check if the data of the first dataModelOne is read correctly
            Assert.Equal("Exit1", dataModelOnes[0].Exit.Name);
            Assert.Equal("Port1", dataModelOnes[0].Port);
            Assert.Equal("UserGroup1", dataModelOnes[0].UserGroup);
            Assert.Equal("Country1", dataModelOnes[0].Country);
            Assert.Equal(1, dataModelOnes[0].MemberId);
            Assert.Equal(new DateOnly(2024, 01, 01), dataModelOnes[0].Date);
            Assert.Equal(5000, dataModelOnes[0].GainAmountOne);
            Assert.Equal(3000, dataModelOnes[0].GainAmountTwo);
            Assert.Equal(500, dataModelOnes[0].Loss);
            Assert.Equal(7500, dataModelOnes[0].Total);

            // Check if the data of the second dataModelOne is read correctly
            Assert.Equal("Exit2", dataModelOnes[1].Exit.Name);
            Assert.Equal("Port2", dataModelOnes[1].Port);
            Assert.Equal("UserGroup2", dataModelOnes[1].UserGroup);
            Assert.Equal("Country2", dataModelOnes[1].Country);
            Assert.Equal(2, dataModelOnes[1].MemberId);
            Assert.Equal(new DateOnly(2024, 01, 02), dataModelOnes[1].Date);
            Assert.Equal(6000, dataModelOnes[1].GainAmountOne);
            Assert.Equal(4000, dataModelOnes[1].GainAmountTwo);
            Assert.Equal(600, dataModelOnes[1].Loss);
            Assert.Equal(9400, dataModelOnes[1].Total);

            // Clean up test file
            File.Delete(testFilePath);
        }

        [Fact]
        public void ParseCsv_InvalidDateData_ReturnsEmptyList()
        {
            // Arrange
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "InvalidDataModelOneTestData.csv");
            CreateTestCsvData(testFilePath, TestDataScenario.InvalidDate);

            // Act
            var dataModelOnes = DataModelOneService.ParseCsv(testFilePath);

            // Assert
            Assert.NotNull(dataModelOnes);
            Assert.Empty(dataModelOnes); // Expecting an empty list due to invalid data

            // Clean up test file
            File.Delete(testFilePath);
        }

        [Fact]
        public void ParseCsv_InvalidGainAmountOneData_ReturnsEmptyList()
        {
            // Arrange
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "InvalidDataModelOneTestData.csv");
            CreateTestCsvData(testFilePath, TestDataScenario.InvalidGainAmountOne);

            // Act
            var dataModelOnes = DataModelOneService.ParseCsv(testFilePath);

            // Assert
            Assert.NotNull(dataModelOnes);
            Assert.Empty(dataModelOnes); // Expecting an empty list due to invalid data

            // Clean up test file
            File.Delete(testFilePath);
        }

        [Fact]
        public void FetchDataModelOneData_NoFilters_ReturnsAllDataModelOneDataOrderedByDate()
        {
            // Arrange
            var contextMock = new Mock<DataManagerDbContext>();
            var dateFrom = new DateOnly(2024, 1, 1);
            var dateTo = new DateOnly(2024, 6, 1);
            var exitId = default(int); // No filter on exitId

            var dataModelOneData = new List<DataModelOne>
            {
                new() { Date = new DateOnly(2024, 1, 5), ExitId = 1 },
                new() { Date = new DateOnly(2024, 2, 10), ExitId = 2 },
            };

            // Mocking the dataModelOnes DbSet in the context
            var mockDataModelOnes = MockDbSetHelper.CreateMockDbSet(dataModelOneData);
            contextMock.Setup(c => c.DataModelOnes).Returns(mockDataModelOnes.Object);

            // Act
            var result = DataModelOneService.FetchDataModelOneData(contextMock.Object, dateFrom, dateTo, exitId);

            // Assert
            // Verify that the result contains all dataModelOnes within the date range, ordered by date
            Assert.Equal(dataModelOneData.OrderBy(a => a.Date), result);
        }

        [Fact]
        public void FetchDataModelOnesData_DateRangeFilter_ReturnsDataModelOnesWithinRangeOrderedByDate()
        {
            // Arrange
            var contextMock = new Mock<DataManagerDbContext>();
            var dateFrom = new DateOnly(2024, 1, 1);
            var dateTo = new DateOnly(2024, 6, 1);
            var exitId = default(int); // No filter on exitId

            var dataModelOneData = new List<DataModelOne>
            {
                new() { Date = new DateOnly(2024, 1, 5), ExitId = 1 },
                new() { Date = new DateOnly(2024, 2, 10), ExitId = 2 },
            };

            // Mocking the dataModelOnes DbSet in the context
            var mockDataModelOnes = MockDbSetHelper.CreateMockDbSet(dataModelOneData);
            contextMock.Setup(c => c.DataModelOnes).Returns(mockDataModelOnes.Object);

            // Act
            var result = DataModelOneService.FetchDataModelOneData(contextMock.Object, dateFrom, dateTo, exitId);

            // Assert
            // Verify that the result contains all dataModelOnes within the date range, ordered by date
            Assert.Equal(dataModelOneData.Where(a => a.Date >= dateFrom && a.Date <= dateTo).OrderBy(a => a.Date), result);
        }

        [Fact]
        public void FetchDataModelOnesData_ExitIdFilter_ReturnsDataModelOnesForSpecificExitOrderedByExitIdThenDate()
        {
            // Arrange
            var contextMock = new Mock<DataManagerDbContext>();
            var dateFrom = default(DateOnly);
            var dateTo = default(DateOnly);
            var exitId = 1; // Example exit ID

            var dataModelOneData = new List<DataModelOne>
            {
                new() { Date = new DateOnly(2024, 1, 5), ExitId = 1 },
                new() { Date = new DateOnly(2024, 2, 10), ExitId = 1 },
            };

            // Mocking the dataModelOnes DbSet in the context
            var mockDataModelOnes = MockDbSetHelper.CreateMockDbSet(dataModelOneData);
            contextMock.Setup(c => c.DataModelOnes).Returns(mockDataModelOnes.Object);

            // Act
            var result = DataModelOneService.FetchDataModelOneData(contextMock.Object, dateFrom, dateTo, exitId);

            // Assert
            // Verify that the result contains dataModelOnes only for the specified exit ID, ordered by exit ID then date
            Assert.Equal(dataModelOneData.Where(a => a.ExitId == exitId).OrderBy(a => a.ExitId).ThenBy(a => a.Date), result);
        }
    }
}

using Moq;
using DataManager.Core;
using DataManager.Core.Services;
using DataManager.Core.DBModels;

namespace DataManager.Test
{
    public class ModelOneServiceTests
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
        public void ParseCsv_DataParsedCorrectly_ReturnsModelOneList()
        {
            // Arrange
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "ValidModelOneTestData.csv");
            CreateTestCsvData(testFilePath, TestDataScenario.ValidData);

            // Act
            var modelOnes = ModelOneService.ParseCsv(testFilePath);

            // Assert
            Assert.NotNull(modelOnes);
            Assert.NotEmpty(modelOnes);
            Assert.Equal(2, modelOnes.Count); // Check if the correct number of ModelOnes are read

            // Check if the data of the first ModelOne is read correctly
            Assert.Equal("Exit1", modelOnes[0].Exit.Name);
            Assert.Equal("Port1", modelOnes[0].Port);
            Assert.Equal("UserGroup1", modelOnes[0].UserGroup);
            Assert.Equal("Country1", modelOnes[0].Country);
            Assert.Equal(1, modelOnes[0].MemberId);
            Assert.Equal(new DateOnly(2024, 01, 01), modelOnes[0].Date);
            Assert.Equal(5000, modelOnes[0].GainAmountOne);
            Assert.Equal(3000, modelOnes[0].GainAmountTwo);
            Assert.Equal(500, modelOnes[0].Loss);
            Assert.Equal(7500, modelOnes[0].Total);

            // Check if the data of the second ModelOne is read correctly
            Assert.Equal("Exit2", modelOnes[1].Exit.Name);
            Assert.Equal("Port2", modelOnes[1].Port);
            Assert.Equal("UserGroup2", modelOnes[1].UserGroup);
            Assert.Equal("Country2", modelOnes[1].Country);
            Assert.Equal(2, modelOnes[1].MemberId);
            Assert.Equal(new DateOnly(2024, 01, 02), modelOnes[1].Date);
            Assert.Equal(6000, modelOnes[1].GainAmountOne);
            Assert.Equal(4000, modelOnes[1].GainAmountTwo);
            Assert.Equal(600, modelOnes[1].Loss);
            Assert.Equal(9400, modelOnes[1].Total);

            // Clean up test file
            File.Delete(testFilePath);
        }

        [Fact]
        public void ParseCsv_InvalidDateData_ReturnsEmptyList()
        {
            // Arrange
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "InvalidModelOneTestData.csv");
            CreateTestCsvData(testFilePath, TestDataScenario.InvalidDate);

            // Act
            var dataModelOnes = ModelOneService.ParseCsv(testFilePath);

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
            string testFilePath = Path.Combine(Directory.GetCurrentDirectory(), "InvalidModelOneTestData.csv");
            CreateTestCsvData(testFilePath, TestDataScenario.InvalidGainAmountOne);

            // Act
            var dataModelOnes = ModelOneService.ParseCsv(testFilePath);

            // Assert
            Assert.NotNull(dataModelOnes);
            Assert.Empty(dataModelOnes); // Expecting an empty list due to invalid data

            // Clean up test file
            File.Delete(testFilePath);
        }

        [Fact]
        public void FetchDataModelOneData_NoFilters_ReturnsAllModelOneDataOrderedByDate()
        {
            // Arrange
            var contextMock = new Mock<DataManagerDbContext>();
            var dateFrom = new DateOnly(2024, 1, 1);
            var dateTo = new DateOnly(2024, 6, 1);
            var exitId = default(int); // No filter on exitId

            var modelOneData = new List<ModelOne>
            {
                new() { Date = new DateOnly(2024, 1, 5), ExitId = 1 },
                new() { Date = new DateOnly(2024, 2, 10), ExitId = 2 },
            };

            // Mocking the dataModelOnes DbSet in the context
            var mockDataModelOnes = MockDbSetHelper.CreateMockDbSet(modelOneData);
            contextMock.Setup(c => c.ModelOnes).Returns(mockDataModelOnes.Object);

            // Act
            var result = ModelOneService.FetchModelOneData(contextMock.Object, dateFrom, dateTo, exitId);

            // Assert
            // Verify that the result contains all ModelOnes within the date range, ordered by date
            Assert.Equal(modelOneData.OrderBy(a => a.Date), result);
        }

        [Fact]
        public void FetchModelOnesData_DateRangeFilter_ReturnsModelOnesWithinRangeOrderedByDate()
        {
            // Arrange
            var contextMock = new Mock<DataManagerDbContext>();
            var dateFrom = new DateOnly(2024, 1, 1);
            var dateTo = new DateOnly(2024, 6, 1);
            var exitId = default(int); // No filter on exitId

            var modelOneData = new List<ModelOne>
            {
                new() { Date = new DateOnly(2024, 1, 5), ExitId = 1 },
                new() { Date = new DateOnly(2024, 2, 10), ExitId = 2 },
            };

            // Mocking the dataModelOnes DbSet in the context
            var mockModelOnes = MockDbSetHelper.CreateMockDbSet(modelOneData);
            contextMock.Setup(c => c.ModelOnes).Returns(mockModelOnes.Object);

            // Act
            var result = ModelOneService.FetchModelOneData(contextMock.Object, dateFrom, dateTo, exitId);

            // Assert
            // Verify that the result contains all ModelOnes within the date range, ordered by date
            Assert.Equal(modelOneData.Where(a => a.Date >= dateFrom && a.Date <= dateTo).OrderBy(a => a.Date), result);
        }

        [Fact]
        public void FetchModelOnesData_ExitIdFilter_ReturnsModelOnesForSpecificExitOrderedByExitIdThenDate()
        {
            // Arrange
            var contextMock = new Mock<DataManagerDbContext>();
            var dateFrom = default(DateOnly);
            var dateTo = default(DateOnly);
            var exitId = 1; // Example exit ID

            var modelOneData = new List<ModelOne>
            {
                new() { Date = new DateOnly(2024, 1, 5), ExitId = 1 },
                new() { Date = new DateOnly(2024, 2, 10), ExitId = 1 },
            };

            // Mocking the ModelOnes DbSet in the context
            var mockModelOnes = MockDbSetHelper.CreateMockDbSet(modelOneData);
            contextMock.Setup(c => c.ModelOnes).Returns(mockModelOnes.Object);

            // Act
            var result = ModelOneService.FetchModelOneData(contextMock.Object, dateFrom, dateTo, exitId);

            // Assert
            // Verify that the result contains ModelOnes only for the specified exit ID, ordered by exit ID then date
            Assert.Equal(modelOneData.Where(a => a.ExitId == exitId).OrderBy(a => a.ExitId).ThenBy(a => a.Date), result);
        }
    }
}

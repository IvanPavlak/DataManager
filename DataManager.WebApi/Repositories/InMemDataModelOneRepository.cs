using DataManager.Core.DBModels;

namespace DataManager.WebApi.Repositories;

public class InMemDataModelOneRepository : IDataModelOnesRepository
{
    static List<Exit> exits =
    [
        new() { Id = 1, Name = "Exit1" },
        new() { Id = 2, Name = "Exit2" },
        new() { Id = 3, Name = "Exit3" }
    ];

    List<DataModelOne> DataModelOnes =
    [
        new() {
            Id = 1,
            ExitId = 1,
            Exit = exits[0],
            Port = "Port1",
            UserGroup = "UserGroup1",
            Country = "Country1",
            MemberId = 1,
            Date = new DateOnly(2024, 01, 01),
            GainAmountOne = 100,
            GainAmountTwo = 200,
            Loss = 50,
            Total = 250
        },
        new() {
            Id = 2,
            ExitId = 2,
            Exit = exits[1],
            Port = "Port2",
            UserGroup = "UserGroup2",
            Country = "Country2",
            MemberId = 2,
            Date = new DateOnly(2024, 01, 02),
            GainAmountOne = 150,
            GainAmountTwo = 250,
            Loss = 50,
            Total = 350
        },
        new() {
            Id = 3,
            ExitId = 3,
            Exit = exits[2],
            Port = "Port3",
            UserGroup = "UserGroup3",
            Country = "Country3",
            MemberId = 3,
            Date = new DateOnly(2024, 01, 03),
            GainAmountOne = 1000,
            GainAmountTwo = 2000,
            Loss = 500,
            Total = 25000
        }
    ];

    public async Task<IEnumerable<DataModelOne>> GetAllDataModelOnesAsync(int pageNumber, int pageSize, string filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await Task.FromResult(FilterDataModelOnes(filter).Skip(skipCount).Take(pageSize));
    }

    public async Task<DataModelOne> GetDataModelOneAsync(int id)
    {
        return await Task.FromResult(DataModelOnes.Find(DataModelOne => DataModelOne.Id == id));
    }

    public async Task CreateDataModelOneAsync(DataModelOne DataModelOne)
    {
        DataModelOne.Id = DataModelOnes.Max(DataModelOne => DataModelOne.Id) + 1;
        DataModelOnes.Add(DataModelOne);

        await Task.CompletedTask;
    }

    public async Task UpdateDataModelOneAsync(DataModelOne updatedDataModelOne)
    {
        var index = DataModelOnes.FindIndex(DataModelOne => DataModelOne.Id == updatedDataModelOne.Id);
        DataModelOnes[index] = updatedDataModelOne;

        await Task.CompletedTask;
    }

    public async Task DeleteDataModelOneAsync(int id)
    {
        var index = DataModelOnes.FindIndex(DataModelOne => DataModelOne.Id == id);
        DataModelOnes.RemoveAt(index);

        await Task.CompletedTask;
    }

    public async Task<int> CountDataModelOneAsync(string filter)
    {
        return await Task.FromResult(FilterDataModelOnes(filter).Count());
    }

    private IEnumerable<DataModelOne> FilterDataModelOnes(string filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return DataModelOnes;
        }

        return DataModelOnes.Where(DataModelOne => DataModelOne.Country.Contains(filter));
    }
}
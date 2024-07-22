using DataManager.Core.DBModels;

namespace DataManager.WebApi.Repositories;

public class InMemModelOneRepository : IModelOnesRepository
{
    static List<Exit> exits =
    [
        new() { Id = 1, Name = "Exit1" },
        new() { Id = 2, Name = "Exit2" },
        new() { Id = 3, Name = "Exit3" }
    ];

    List<ModelOne> modelOnes =
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

    public async Task<IEnumerable<ModelOne>> GetAllModelOnesAsync(int pageNumber, int pageSize, string filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await Task.FromResult(FilterModelOnes(filter).Skip(skipCount).Take(pageSize));
    }

    public async Task<ModelOne> GetModelOneAsync(int id)
    {
        return await Task.FromResult(modelOnes.Find(ModelOne => ModelOne.Id == id));
    }

    public async Task CreateModelOneAsync(ModelOne ModelOne)
    {
        ModelOne.Id = modelOnes.Max(ModelOne => ModelOne.Id) + 1;
        modelOnes.Add(ModelOne);

        await Task.CompletedTask;
    }

    public async Task UpdateModelOneAsync(ModelOne updatedModelOne)
    {
        var index = modelOnes.FindIndex(ModelOne => ModelOne.Id == updatedModelOne.Id);
        modelOnes[index] = updatedModelOne;

        await Task.CompletedTask;
    }

    public async Task DeleteModelOneAsync(int id)
    {
        var index = modelOnes.FindIndex(ModelOne => ModelOne.Id == id);
        modelOnes.RemoveAt(index);

        await Task.CompletedTask;
    }

    public async Task<int> CountModelOneAsync(string filter)
    {
        return await Task.FromResult(FilterModelOnes(filter).Count());
    }

    private IEnumerable<ModelOne> FilterModelOnes(string filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return modelOnes;
        }

        return modelOnes.Where(ModelOne => ModelOne.Country.Contains(filter));
    }
}
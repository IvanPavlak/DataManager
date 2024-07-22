using DataManager.Core.DBModels;

namespace DataManager.WebApi.Repositories;

public class InMemModelTwosRepository : IModelTwosRepository
{
    static List<Exit> exits =
    [
        new() { Id = 1, Name = "Exit1" },
        new() { Id = 2, Name = "Exit2" },
        new() { Id = 3, Name = "Exit3" }
    ];

    List<ModelTwo> modelTwos =
    [
        new() {
            Id = 1,
            ExitId = 1,
            Exit = exits[0],
            PeriodStartDate = new DateOnly(2024, 01, 01),
            PeriodEndDate = new DateTime(2024, 01, 02, 00, 00, 00),
            GainAmountThree = 500
        },
        new() {
            Id = 2,
            ExitId = 2,
            Exit = exits[1],
            PeriodStartDate = new DateOnly(2024, 01, 02),
            PeriodEndDate = new DateTime(2024, 01, 03, 00, 00, 00),
            GainAmountThree = 1000
        },
        new() {
            Id = 3,
            ExitId = 3,
            Exit = exits[2],
            PeriodStartDate = new DateOnly(2024, 01, 03),
            PeriodEndDate = new DateTime(2024, 01, 04, 00, 00, 00),
            GainAmountThree = 2000
        }
    ];

    public async Task<IEnumerable<ModelTwo>> GetAllModelTwosAsync(int PageNumber, int PageSize, string Filter)
    {
        var skipCount = (PageNumber - 1) * PageSize;

        return await Task.FromResult(modelTwos.Skip(skipCount).Take(PageSize));
    }

    public async Task<ModelTwo> GetModelTwoAsync(int id)
    {
        return await Task.FromResult(modelTwos.Find(ModelTwo => ModelTwo.Id == id));
    }

    public async Task CreateModelTwoAsync(ModelTwo ModelTwo)
    {
        ModelTwo.Id = modelTwos.Max(ModelTwo => ModelTwo.Id) + 1;
        modelTwos.Add(ModelTwo);

        await Task.CompletedTask;
    }

    public async Task UpdateModelTwoAsync(ModelTwo updatedModelTwo)
    {
        var index = modelTwos.FindIndex(ModelTwo => ModelTwo.Id == updatedModelTwo.Id);
        modelTwos[index] = updatedModelTwo;

        await Task.CompletedTask;
    }

    public async Task DeleteModelTwoAsync(int id)
    {
        var index = modelTwos.FindIndex(ModelTwo => ModelTwo.Id == id);
        modelTwos.RemoveAt(index);

        await Task.CompletedTask;
    }

    public async Task<int> CountModelTwoAsync()
    {
        return await Task.FromResult(modelTwos.Count());
    }
}
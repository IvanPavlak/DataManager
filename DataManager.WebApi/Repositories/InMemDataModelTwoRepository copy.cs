using DataManager.Core.DBModels;

namespace DataManager.WebApi.Repositories;

public class InMemDataModelTwosRepository : IDataModelTwosRepository
{
    static List<Exit> exits =
    [
        new() { Id = 1, Name = "Exit1" },
        new() { Id = 2, Name = "Exit2" },
        new() { Id = 3, Name = "Exit3" }
    ];

    List<DataModelTwo> DataModelTwos =
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

    public async Task<IEnumerable<DataModelTwo>> GetAllDataModelTwosAsync(int PageNumber, int PageSize, string Filter)
    {
        var skipCount = (PageNumber - 1) * PageSize;

        return await Task.FromResult(DataModelTwos.Skip(skipCount).Take(PageSize));
    }

    public async Task<DataModelTwo> GetDataModelTwoAsync(int id)
    {
        return await Task.FromResult(DataModelTwos.Find(DataModelTwo => DataModelTwo.Id == id));
    }

    public async Task CreateDataModelTwoAsync(DataModelTwo DataModelTwo)
    {
        DataModelTwo.Id = DataModelTwos.Max(DataModelTwo => DataModelTwo.Id) + 1;
        DataModelTwos.Add(DataModelTwo);

        await Task.CompletedTask;
    }

    public async Task UpdateDataModelTwoAsync(DataModelTwo updatedDataModelTwo)
    {
        var index = DataModelTwos.FindIndex(DataModelTwo => DataModelTwo.Id == updatedDataModelTwo.Id);
        DataModelTwos[index] = updatedDataModelTwo;

        await Task.CompletedTask;
    }

    public async Task DeleteDataModelTwoAsync(int id)
    {
        var index = DataModelTwos.FindIndex(DataModelTwo => DataModelTwo.Id == id);
        DataModelTwos.RemoveAt(index);

        await Task.CompletedTask;
    }

    public async Task<int> CountDataModelTwoAsync()
    {
        return await Task.FromResult(DataModelTwos.Count());
    }
}
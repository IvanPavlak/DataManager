using DataManager.Core;
using DataManager.Core.DBModels;
using Microsoft.EntityFrameworkCore;

namespace DataManager.WebApi.Repositories;

public class EntityFrameworkRepository : IDataModelOnesRepository, IDataModelTwosRepository
{
    private readonly DataManagerDbContext dbContext;

    private readonly ILogger<EntityFrameworkRepository> logger;

    public EntityFrameworkRepository(DataManagerDbContext dbContext, ILogger<EntityFrameworkRepository> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<IEnumerable<DataModelOne>> GetAllDataModelOnesAsync(int pageNumber, int pageSize, string filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await FilterDataModelOnes(filter)
                    .OrderBy(dataModelOne => dataModelOne.Id)
                    .Skip(skipCount)
                    .Take(pageSize)
                    .AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<DataModelTwo>> GetAllDataModelTwosAsync(int pageNumber, int pageSize, string filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await dbContext.DataModelTwos
                              .OrderBy(dataModelTwo => dataModelTwo.Id)
                              .Skip(skipCount)
                              .Take(pageSize)
                              .AsNoTracking().ToListAsync();
    }

    public async Task<DataModelOne> GetDataModelOneAsync(int id)
    {
        return await dbContext.DataModelOnes.FindAsync(id);
    }

    public async Task<DataModelTwo> GetDataModelTwoAsync(int id)
    {
        return await dbContext.DataModelTwos.FindAsync(id);
    }

    public async Task CreateDataModelOneAsync(DataModelOne dataModelOne)
    {
        dbContext.DataModelOnes.Add(dataModelOne);
        await dbContext.SaveChangesAsync();
    }

    public async Task CreateDataModelTwoAsync(DataModelTwo dataModelTwo)
    {
        dbContext.DataModelTwos.Add(dataModelTwo);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateDataModelOneAsync(DataModelOne updatedDataModelOne)
    {
        dbContext.DataModelOnes.Update(updatedDataModelOne);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateDataModelTwoAsync(DataModelTwo updatedDataModelTwo)
    {
        dbContext.DataModelTwos.Update(updatedDataModelTwo);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteDataModelOneAsync(int id)
    {
        await dbContext.DataModelOnes.Where(dataModelOne => dataModelOne.Id == id)
                       .ExecuteDeleteAsync();
    }

    public async Task DeleteDataModelTwoAsync(int id)
    {
        await dbContext.DataModelTwos.Where(dataModelTwo => dataModelTwo.Id == id)
                       .ExecuteDeleteAsync();
    }

    public async Task<int> CountDataModelOneAsync(string filter)
    {
        return await FilterDataModelOnes(filter).CountAsync();
    }

    public async Task<int> CountDataModelTwoAsync()
    {
        return await dbContext.DataModelTwos.CountAsync();
    }

    private IQueryable<DataModelOne> FilterDataModelOnes(string filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return dbContext.DataModelOnes;
        }

        return dbContext.DataModelOnes
                        .Where(DataModelOne => DataModelOne.Country.Contains(filter)); 
    }
}
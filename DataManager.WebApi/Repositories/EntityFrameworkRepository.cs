using DataManager.Core;
using DataManager.Core.DBModels;
using Microsoft.EntityFrameworkCore;

namespace DataManager.WebApi.Repositories;

public class EntityFrameworkRepository : IModelOnesRepository, IModelTwosRepository
{
    private readonly DataManagerDbContext dbContext;

    private readonly ILogger<EntityFrameworkRepository> logger;

    public EntityFrameworkRepository(DataManagerDbContext dbContext, ILogger<EntityFrameworkRepository> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<IEnumerable<ModelOne>> GetAllModelOnesAsync(int pageNumber, int pageSize, string filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await FilterModelOnes(filter)
                    .OrderBy(ModelOne => ModelOne.Id)
                    .Skip(skipCount)
                    .Take(pageSize)
                    .AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<ModelTwo>> GetAllModelTwosAsync(int pageNumber, int pageSize, string filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await dbContext.ModelTwos
                              .OrderBy(ModelTwo => ModelTwo.Id)
                              .Skip(skipCount)
                              .Take(pageSize)
                              .AsNoTracking().ToListAsync();
    }

    public async Task<ModelOne> GetModelOneAsync(int id)
    {
        return await dbContext.ModelOnes.FindAsync(id);
    }

    public async Task<ModelTwo> GetModelTwoAsync(int id)
    {
        return await dbContext.ModelTwos.FindAsync(id);
    }

    public async Task CreateModelOneAsync(ModelOne ModelOne)
    {
        dbContext.ModelOnes.Add(ModelOne);
        await dbContext.SaveChangesAsync();
    }

    public async Task CreateModelTwoAsync(ModelTwo ModelTwo)
    {
        dbContext.ModelTwos.Add(ModelTwo);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateModelOneAsync(ModelOne updatedModelOne)
    {
        dbContext.ModelOnes.Update(updatedModelOne);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateModelTwoAsync(ModelTwo updatedModelTwo)
    {
        dbContext.ModelTwos.Update(updatedModelTwo);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteModelOneAsync(int id)
    {
        await dbContext.ModelOnes.Where(ModelOne => ModelOne.Id == id)
                       .ExecuteDeleteAsync();
    }

    public async Task DeleteModelTwoAsync(int id)
    {
        await dbContext.ModelTwos.Where(ModelTwo => ModelTwo.Id == id)
                       .ExecuteDeleteAsync();
    }

    public async Task<int> CountModelOneAsync(string filter)
    {
        return await FilterModelOnes(filter).CountAsync();
    }

    public async Task<int> CountModelTwoAsync()
    {
        return await dbContext.ModelTwos.CountAsync();
    }

    private IQueryable<ModelOne> FilterModelOnes(string filter)
    {
        if (string.IsNullOrEmpty(filter))
        {
            return dbContext.ModelOnes;
        }

        return dbContext.ModelOnes
                        .Where(ModelOne => ModelOne.Country.Contains(filter));
    }
}
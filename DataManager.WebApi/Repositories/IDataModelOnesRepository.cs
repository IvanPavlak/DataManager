using DataManager.Core.DBModels;

namespace DataManager.WebApi.Repositories;

public interface IModelOnesRepository
{
    Task CreateModelOneAsync(ModelOne ModelOne);
    Task DeleteModelOneAsync(int id);
    Task<ModelOne> GetModelOneAsync(int id);
    Task<IEnumerable<ModelOne>> GetAllModelOnesAsync(int pageNumber, int pageSize, string filter = null);
    Task UpdateModelOneAsync(ModelOne updatedModelOnes);
    Task<int> CountModelOneAsync(string filter = null);
}
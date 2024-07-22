using DataManager.Core.DBModels;

namespace DataManager.WebApi.Repositories;

public interface IModelTwosRepository
{
    Task CreateModelTwoAsync(ModelTwo ModelTwo);
    Task DeleteModelTwoAsync(int id);
    Task<ModelTwo> GetModelTwoAsync(int id);
    Task<IEnumerable<ModelTwo>> GetAllModelTwosAsync(int pageNumber, int pageSize, string filter = null);
    Task UpdateModelTwoAsync(ModelTwo updatedModelTwo);
    Task<int> CountModelTwoAsync();
}
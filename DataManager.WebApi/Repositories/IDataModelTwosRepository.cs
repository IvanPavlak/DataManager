using DataManager.Core.DBModels;

namespace DataManager.WebApi.Repositories;

public interface IDataModelTwosRepository
{
    Task CreateDataModelTwoAsync(DataModelTwo dataModelTwo);
    Task DeleteDataModelTwoAsync(int id);
    Task<DataModelTwo> GetDataModelTwoAsync(int id);
    Task<IEnumerable<DataModelTwo>> GetAllDataModelTwosAsync(int pageNumber, int pageSize, string filter = null);
    Task UpdateDataModelTwoAsync(DataModelTwo updatedDataModelTwo);
    Task<int> CountDataModelTwoAsync();
}
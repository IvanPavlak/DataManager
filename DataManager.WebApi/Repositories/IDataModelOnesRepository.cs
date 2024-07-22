using DataManager.Core.DBModels;

namespace DataManager.WebApi.Repositories;

public interface IDataModelOnesRepository
{
    Task CreateDataModelOneAsync(DataModelOne dataModelOne);
    Task DeleteDataModelOneAsync(int id);
    Task<DataModelOne> GetDataModelOneAsync(int id);
    Task<IEnumerable<DataModelOne>> GetAllDataModelOnesAsync(int pageNumber, int pageSize, string filter = null);
    Task UpdateDataModelOneAsync(DataModelOne updatedDataModelOnes);
    Task<int> CountDataModelOneAsync(string filter = null);
}
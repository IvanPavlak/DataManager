namespace DataManager.Core.DBModels;

public class Exit : Entity
{
    public string Name { get; set; }
    public virtual ICollection<DataModelOne> DataModelOnes { get; set; }
    public virtual ICollection<DataModelTwo> DataModelTwos { get; set; }
}
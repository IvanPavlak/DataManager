namespace DataManager.Core.DBModels;

public class Exit : Entity
{
    public string Name { get; set; }
    public virtual ICollection<ModelOne> ModelOnes { get; set; }
    public virtual ICollection<ModelTwo> ModelTwos { get; set; }
}
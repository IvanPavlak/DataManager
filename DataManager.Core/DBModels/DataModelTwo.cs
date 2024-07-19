namespace DataManager.Core.DBModels;

public class DataModelTwo : Entity
{
    public int ExitId { get; set; }
    public virtual Exit Exit { get; set; }
    public DateOnly PeriodStartDate { get; set; }
    public DateTime PeriodEndDate { get; set; }
    public int GainAmountThree { get; set; }
}
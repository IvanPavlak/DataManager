namespace DataManager.Core.DBModels;

public class DataModelTwo : Entity
{
    public DateOnly PeriodStartDate { get; set; }
    public string Exit { get; set; }
    public int GainAmountThree { get; set; }
    public DateTimeOffset PeriodEndDate { get; set; }
}
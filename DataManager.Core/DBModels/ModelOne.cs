
namespace DataManager.Core.DBModels;

public class ModelOne : Entity
{
    public int ExitId { get; set; }
    public virtual Exit Exit { get; set; }
    public string Port { get; set; }
    public string UserGroup { get; set; }
    public string Country { get; set; }
    public int MemberId { get; set; }
    public DateOnly Date { get; set; }
    public int GainAmountOne { get; set; }
    public int GainAmountTwo { get; set; }
    public int Loss { get; set; }
    public int Total { get; set; }
}
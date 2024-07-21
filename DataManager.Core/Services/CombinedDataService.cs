namespace DataManager.Core.Services;

public class CombinedDataService
{
    public DateOnly Date { get; set; }
    public int ExitId { get; set; }
    public string ExitName { get; set; }
    public double TotalDataModelOne { get; set; }
    public double TotalDataModelTwo { get; set; }
    public double Ratio { get; set; }

    public static List<CombinedDataService> FetchCombinedData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        var dataModelOneData = DataModelOneService.FetchDataModelOneData(context, dateFrom, dateTo, exitId).ToList();
        var dataModelTwoData = DataModelTwoService.FetchDataModelTwoData(context, dateFrom, dateTo, exitId).ToList();
        var exits = context.Exits.ToDictionary(e => e.Id, e => e.Name);

        var dataModelTwoDict = dataModelTwoData
            .GroupBy(f => new { f.PeriodStartDate, f.ExitId })
            .ToDictionary(g => g.Key, g => g.First());

        var combinedData = new List<CombinedDataService>();

        foreach (var dataModelOne in dataModelOneData)
        {
            var key = new { PeriodStartDate = dataModelOne.Date, dataModelOne.ExitId };

            if (dataModelTwoDict.TryGetValue(key, out var matchingDataModelTwo))
            {
                string exitName = exits.TryGetValue(dataModelOne.ExitId, out var name) ? name : "Unknown";

                double totalDataModelOne = dataModelOne.Total;
                double totalDataModelTwo = matchingDataModelTwo.GainAmountThree;
                double ratio = totalDataModelTwo != 0 ? totalDataModelOne / totalDataModelTwo * 100 : 0;
                ratio = double.IsNaN(ratio) || double.IsInfinity(ratio) ? 0 : ratio;

                var combinedItem = new CombinedDataService
                {
                    Date = dataModelOne.Date,
                    ExitId = dataModelOne.ExitId,
                    ExitName = exitName,
                    TotalDataModelOne = totalDataModelOne,
                    TotalDataModelTwo = totalDataModelTwo,
                    Ratio = ratio
                };

                combinedData.Add(combinedItem);
            }
        }
        return combinedData;
    }
}
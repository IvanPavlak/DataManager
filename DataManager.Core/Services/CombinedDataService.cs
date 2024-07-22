namespace DataManager.Core.Services;

public class CombinedDataService
{
    public DateOnly Date { get; set; }
    public int ExitId { get; set; }
    public string ExitName { get; set; }
    public double TotalModelOne { get; set; }
    public double TotalModelTwo { get; set; }
    public double Ratio { get; set; }

    public static List<CombinedDataService> FetchCombinedData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        var modelOneData = ModelOneService.FetchModelOneData(context, dateFrom, dateTo, exitId).ToList();
        var modelTwoData = ModelTwoService.FetchModelTwoData(context, dateFrom, dateTo, exitId).ToList();
        var exits = context.Exits.ToDictionary(e => e.Id, e => e.Name);

        var modelTwoDict = modelTwoData
            .GroupBy(f => new { f.PeriodStartDate, f.ExitId })
            .ToDictionary(g => g.Key, g => g.First());

        var combinedData = new List<CombinedDataService>();

        foreach (var modelOne in modelOneData)
        {
            var key = new { PeriodStartDate = modelOne.Date, modelOne.ExitId };

            if (modelTwoDict.TryGetValue(key, out var matchingModelTwo))
            {
                string exitName = exits.TryGetValue(modelOne.ExitId, out var name) ? name : "Unknown";

                double totalModelOne = modelOne.Total;
                double totalModelTwo = matchingModelTwo.GainAmountThree;
                double ratio = totalModelTwo != 0 ? totalModelOne / totalModelTwo * 100 : 0;
                ratio = double.IsNaN(ratio) || double.IsInfinity(ratio) ? 0 : ratio;

                var combinedItem = new CombinedDataService
                {
                    Date = modelOne.Date,
                    ExitId = modelOne.ExitId,
                    ExitName = exitName,
                    TotalModelOne = totalModelOne,
                    TotalModelTwo = totalModelTwo,
                    Ratio = ratio
                };

                combinedData.Add(combinedItem);
            }
        }
        return combinedData;
    }
}
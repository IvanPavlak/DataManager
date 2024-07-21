using DataManager.Core;
using DataManager.Core.DBModels;
using DataManager.Core.Services;
using Spectre.Console;

namespace DataManager.Console;

public class DataManagerDisplayData
{

    private static void DataModelOneDataTable(List<DataModelOne> dataModelOnes, DataManagerDbContext dbContext, int pageNumber = 1, int pageSize = 15)
    {
        AnsiConsole.Clear(); // By commenting this line, table pages are rendered below each other
        PrettifyConsole.Title("DataModelOne Data", "up");

        var totalItems = dataModelOnes.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        if (totalItems == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No data available![/]");
            return;
        }

        var startIndex = (pageNumber - 1) * pageSize;
        var endIndex = Math.Min(startIndex + pageSize, totalItems);

        var table = new Table();
        table.Centered();
        table.AddColumn("[bold red]Exit[/]");
        table.AddColumn("[bold dodgerblue1]User Group[/]");
        table.AddColumn("[bold dodgerblue1]Country[/]");
        table.AddColumn("[bold dodgerblue1]Member ID[/]");
        table.AddColumn("[bold dodgerblue1]Date[/]");
        table.AddColumn("[bold red]Gain Amount One[/]");
        table.AddColumn("[bold red]Gain Amount Two[/]");
        table.AddColumn("[bold red]Loss Amount[/]");
        table.AddColumn("[bold red]Total Amount[/]");
        table.Columns[0].Centered();
        table.Columns[1].Centered();
        table.Columns[2].Centered();
        table.Columns[3].Centered();
        table.Columns[4].Centered();
        table.Columns[5].Centered();
        table.Columns[6].Centered();
        table.Columns[7].Centered();
        table.Columns[8].Centered();
        table.Border(TableBorder.Rounded);

        for (int i = startIndex; i < endIndex; i++)
        {
            var item = dataModelOnes[i];
            if (item is DataModelOne dataModelOne)
            {
                var exitName = dbContext.Exits
                    .Where(e => e.Id == dataModelOne.ExitId)
                    .Select(e => e.Name)
                    .FirstOrDefault();

                table.AddRow(exitName ?? "Unknown",
                             dataModelOne.UserGroup.ToString(),
                             dataModelOne.Country.ToString(),
                             dataModelOne.MemberId.ToString(),
                             dataModelOne.Date.ToString("dd.MM.yyyy"),
                             dataModelOne.GainAmountOne.ToString(),
                             dataModelOne.GainAmountTwo.ToString(),
                             dataModelOne.Loss.ToString(),
                             dataModelOne.Total.ToString());
            }
        }

        AnsiConsole.Write(table);
        PrettifyConsole.Pagination(dataModelOnes, dbContext, pageNumber, pageSize, DataModelOneDataTable);
    }

    public static void DisplayDataModelOneData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        var data = DataModelOneService.FetchDataModelOneData(context, dateFrom, dateTo, exitId);
        DataModelOneDataTable(data, context);
    }

    private static void DataModelTwoDataTable(List<DataModelTwo> dataModelTwos, DataManagerDbContext dbContext, int pageNumber = 1, int pageSize = 15)
    {
        AnsiConsole.Clear(); // By commenting this line, table pages are rendered below each other
        PrettifyConsole.Title("DataModelTwo Data", "up");

        var totalItems = dataModelTwos.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        if (totalItems == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No data available![/]");
            return;
        }

        var startIndex = (pageNumber - 1) * pageSize;
        var endIndex = Math.Min(startIndex + pageSize, totalItems);

        var table = new Table();
        table.Centered();
        table.AddColumn("[bold red]Exit[/]");
        table.AddColumn("[bold dodgerblue1]Period Start Date[/]");
        table.AddColumn("[bold dodgerblue1]Period End Date[/]");
        table.AddColumn("[bold red]Gain Amount Three[/]");
        table.Columns[0].Centered();
        table.Columns[1].Centered();
        table.Columns[2].Centered();
        table.Columns[3].Centered();
        table.Border(TableBorder.Rounded);

        for (int i = startIndex; i < endIndex; i++)
        {
            var dataModelTwo = dataModelTwos[i];
            var exitName = dbContext.Exits
                .Where(e => e.Id == dataModelTwo.ExitId)
                .Select(e => e.Name)
                .FirstOrDefault();

            table.AddRow(exitName ?? "Unknown",
                         dataModelTwo.PeriodStartDate.ToString("dd.MM.yyyy"),
                         dataModelTwo.PeriodEndDate.ToString("dd.MM.yyyy"),
                         dataModelTwo.GainAmountThree.ToString());
        }

        AnsiConsole.Write(table);
        PrettifyConsole.Pagination(dataModelTwos, dbContext, pageNumber, pageSize, DataModelTwoDataTable);
    }

    public static void DisplayDataModelTwoData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        var dataModelTwos = DataModelTwoService.FetchDataModelTwoData(context, dateFrom, dateTo, exitId);
        DataModelTwoDataTable(dataModelTwos, context);
    }

    private static void CombinedDataTable(List<CombinedDataService> combinedData, DataManagerDbContext dbContext, int pageNumber = 1, int pageSize = 15)
    {
        AnsiConsole.Clear(); // By commenting this line, table pages are rendered below each other
        PrettifyConsole.Title("Combined Data", "up");

        var totalItems = combinedData.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        if (totalItems == 0)
        {
            AnsiConsole.MarkupLine("[bold red]No data available![/]");
            return;
        }

        var startIndex = (pageNumber - 1) * pageSize;
        var endIndex = Math.Min(startIndex + pageSize, totalItems);

        var table = new Table();
        table.Centered();
        table.AddColumn("[bold red]Exit[/]");
        table.AddColumn("[bold dodgerblue1]Date[/]");
        table.AddColumn("[bold dodgerblue1]Total Gain Amount One[/]");
        table.AddColumn("[bold dodgerblue1]Total Gain Amount Three[/]");
        table.AddColumn("[bold red]Total Gain Amount Three / Total Gain Amount One[/]");
        table.Columns[0].Centered();
        table.Columns[1].Centered();
        table.Columns[2].Centered();
        table.Columns[3].Centered();
        table.Columns[4].Centered();
        table.Border(TableBorder.Rounded);

        for (int i = startIndex; i < endIndex; i++)
        {
            var item = combinedData[i];
            var exitName = dbContext.Exits
                .Where(e => e.Id == item.ExitId)
                .Select(e => e.Name)
                .FirstOrDefault();

            table.AddRow(exitName ?? "Unknown",
                         item.Date.ToString("dd.MM.yyyy"),
                         item.TotalDataModelTwo.ToString(),
                         item.TotalDataModelOne.ToString(),
                         $"{item.Ratio:F4}");
        }

        AnsiConsole.Write(table);
        PrettifyConsole.Pagination(combinedData, dbContext, pageNumber, pageSize, CombinedDataTable);
    }

    public static void DisplayCombinedData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        var combinedData = CombinedDataService.FetchCombinedData(context, dateFrom, dateTo, exitId);
        CombinedDataTable(combinedData, context);
    }
}
using Spectre.Console;
using DataManager.Core;
using DataManager.Core.DBModels;
using DataManager.Core.Services;

namespace DataManager.Console;

public class DataManagerDisplay
{

    private static void ModelOneDataTable(List<ModelOne> modelOnes, DataManagerDbContext dbContext, int pageNumber = 1, int pageSize = 15)
    {
        AnsiConsole.Clear(); // By commenting this line, table pages are rendered below each other
        PrettifyConsole.Title("ModelOne Data", "up");

        var totalItems = modelOnes.Count;
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
            var item = modelOnes[i];
            if (item is ModelOne modelOne)
            {
                var exitName = dbContext.Exits
                    .Where(e => e.Id == modelOne.ExitId)
                    .Select(e => e.Name)
                    .FirstOrDefault();

                table.AddRow(exitName ?? "Unknown",
                             modelOne.UserGroup.ToString(),
                             modelOne.Country.ToString(),
                             modelOne.MemberId.ToString(),
                             modelOne.Date.ToString("dd.MM.yyyy"),
                             modelOne.GainAmountOne.ToString(),
                             modelOne.GainAmountTwo.ToString(),
                             modelOne.Loss.ToString(),
                             modelOne.Total.ToString());
            }
        }

        AnsiConsole.Write(table);
        PrettifyConsole.Pagination(modelOnes, dbContext, pageNumber, pageSize, ModelOneDataTable);
    }

    public static void DisplayModelOneData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        var data = ModelOneService.FetchModelOneData(context, dateFrom, dateTo, exitId);
        ModelOneDataTable(data, context);
    }

    private static void ModelTwoDataTable(List<ModelTwo> modelTwos, DataManagerDbContext dbContext, int pageNumber = 1, int pageSize = 15)
    {
        AnsiConsole.Clear(); // By commenting this line, table pages are rendered below each other
        PrettifyConsole.Title("ModelTwo Data", "up");

        var totalItems = modelTwos.Count;
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
            var modelTwo = modelTwos[i];
            var exitName = dbContext.Exits
                .Where(e => e.Id == modelTwo.ExitId)
                .Select(e => e.Name)
                .FirstOrDefault();

            table.AddRow(exitName ?? "Unknown",
                         modelTwo.PeriodStartDate.ToString("dd.MM.yyyy"),
                         modelTwo.PeriodEndDate.ToString("dd.MM.yyyy"),
                         modelTwo.GainAmountThree.ToString());
        }

        AnsiConsole.Write(table);
        PrettifyConsole.Pagination(modelTwos, dbContext, pageNumber, pageSize, ModelTwoDataTable);
    }

    public static void DisplayModelTwoData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        var modelTwos = ModelTwoService.FetchModelTwoData(context, dateFrom, dateTo, exitId);
        ModelTwoDataTable(modelTwos, context);
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
                         item.TotalModelTwo.ToString(),
                         item.TotalModelOne.ToString(),
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
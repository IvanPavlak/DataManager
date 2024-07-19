using DataManager.Console;
using DataManager.Core;
using MetForecasting.BDM.Core;
using Spectre.Console;

using (var dbContext = new DataManagerDbContext())
{
    var dataManagerService = new DataManagerService(dbContext);

    PrettifyConsole.Title("MetForecastingBDM");

    AnsiConsole.MarkupLine($"[bold red]-> DELETING EXISTING DATABASE! FOR TESTING ONLY! <-[/]");
    dbContext.Database.EnsureDeleted();
    PrettifyConsole.CreateAndDisplayLine("dodgerblue1");

    string csvFilePath = Path.Combine("E:/VSCode/GitHub/Data_Manager/DataManager.Core/Database/Data/DataModelOne_1k_rows.csv");
    string xlsxFilePath = Path.Combine("E:/VSCode/GitHub/Data_Manager/DataManager.Core/Database/Data/DataModelOne_1k_rows.xlsx");

    DataManagerPrompts.ConsoleAppStartPrompt(dataManagerService, csvFilePath, xlsxFilePath, dbContext);
}
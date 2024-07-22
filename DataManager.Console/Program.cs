using DataManager.Console;
using DataManager.Core;
using DataManager.Core.Services;
using Spectre.Console;

using (var dbContext = new DataManagerDbContext())
{
    var dataManagerService = new DataManagerService(dbContext);

    PrettifyConsole.Title("DataManagerDB");

    AnsiConsole.MarkupLine($"[bold red]-> DELETING EXISTING DATABASE! FOR TESTING ONLY! <-[/]");
    dbContext.Database.EnsureDeleted();
    PrettifyConsole.CreateAndDisplayLine("dodgerblue1");

    string csvFilePath = Path.Combine("E:/VSCode/GitHub/Data_Manager/DataManager.Core/Database/Data/ModelOne_1k_rows.csv");
    string xlsxFilePath = Path.Combine("E:/VSCode/GitHub/Data_Manager/DataManager.Core/Database/Data/ModelTwo_1k_rows.xlsx");

    DataManagerPrompts.ConsoleAppStartPrompt(dataManagerService, csvFilePath, xlsxFilePath, dbContext);
}
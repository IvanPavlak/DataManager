using DataManager.Core;
using DataManager.Console;
using DataManager.Core.Services;

using (var dbContext = new DataManagerDbContext())
{
    var dataManagerService = new DataManagerService(dbContext);

    PrettifyConsole.Title("DataManager");

    string csvFilePath = Path.Combine("E:/VSCode/GitHub/Data_Manager/DataManager.Core/Database/Data/ModelOne_1k_rows.csv");
    string xlsxFilePath = Path.Combine("E:/VSCode/GitHub/Data_Manager/DataManager.Core/Database/Data/ModelTwo_1k_rows.xlsx");

    DataManagerPrompts.ConsoleAppStartPrompt(dataManagerService, csvFilePath, xlsxFilePath, dbContext);
}
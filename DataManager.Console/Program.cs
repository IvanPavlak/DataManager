using DataManager.Core;
using DataManager.Console;
using DataManager.Core.Services;
using DataManager.Core.Database.Data;

using (var dbContext = new DataManagerDbContext())
{
    var dataManagerService = new DataManagerService(dbContext);

    PrettifyConsole.Title("DataManager");

    var (csvFilePath, xlsxFilePath) = FileLocator.GetFilePaths();

    DataManagerPrompts.ConsoleAppStartPrompt(dataManagerService, csvFilePath, xlsxFilePath, dbContext);
}
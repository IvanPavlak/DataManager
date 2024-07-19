using CsvHelper;
using ClosedXML.Excel;
using Spectre.Console;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using DataManager.Core.DBModels;
using DataManager.Core.DbValidation;
using DataManager.Core;
using DataManager.Core.Services;

namespace MetForecasting.BDM.Core;

public class DataManagerService
{
    private readonly DataManagerDbContext _dbContext;

    public DataManagerService(DataManagerDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public enum DataType
    {
        dataModelOne,
        dataModelTwo,
        CombinedData
    }

    public void ImportToDatabase(string filePath, DataType dataType)
    {
        _dbContext.Database.Migrate();

        Dictionary<string, Exit> exitByName = [];

        int exitsRowCountBeforeImport = _dbContext.Exits.Count();

        switch (dataType)
        {
            case DataType.dataModelOne:
                var dataModelOnes = DataModelOneService.ParseCsv(filePath);
                ImportExits(dataModelOnes.Select(a => a.Exit).ToList(), exitByName);
                DataModelOneService.ImportDataModelOnes(_dbContext, dataModelOnes, exitByName);
                break;
            case DataType.dataModelTwo:
                var dataModelTwos = DataModelTwoService.ParseXlsx(filePath);
                ImportExits(dataModelTwos.Select(f => f.Exit).ToList(), exitByName);
                DataModelTwoService.ImportDataModelTwos(_dbContext, dataModelTwos, exitByName);
                break;
            default:
                throw new ArgumentException("Invalid data type specified!");
        }

        _dbContext.SaveChanges();

        int exitsRowCountAfterImport = _dbContext.Exits.Count();
        int newExitsAdded = exitsRowCountAfterImport - exitsRowCountBeforeImport;

        AnsiConsole.MarkupLine($"\n[bold green1]=>[/] [bold red]{Path.GetFileName(filePath)}[/] [bold green1]data imported successfully![/]");
        AnsiConsole.MarkupLine($"[bold green1]    New [bold red]exits[/] added:[/] [bold red]{newExitsAdded}[/]");
    }

    private void ImportExits(List<Exit> exits, Dictionary<string, Exit> exitByName)
    {
        var exitValidator = new ExitValidator();

        foreach (var exit in exits)
        {
            var validationResult = exitValidator.Validate(exit);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[bold red]=> Validation error for Exit: {error.ErrorMessage}[/]");
                }
                continue;
            }
            if (!exitByName.TryGetValue(exit.Name, out Exit existingExit))
            {
                existingExit = _dbContext.Exits.FirstOrDefault(e => e.Name == exit.Name);

                if (existingExit == null)
                {
                    existingExit = exit;
                    exitByName.Add(existingExit.Name, existingExit);
                    _dbContext.Exits.Add(existingExit);
                    AnsiConsole.MarkupLine($"[bold dodgerblue1] New exit [bold green1]'{existingExit.Name}'[/] added to database![/]");
                }
                else
                {
                    exitByName.Add(existingExit.Name, existingExit);
                }
            }
        }
    }

    public static void ExportToCsv<T>(List<T> data)
    {
        try
        {
            string currentDateTime = DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
            string fileName = $"DataManager_{currentDateTime}.csv";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(desktopPath, "DataManager", fileName);

            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var writer = new StreamWriter(fullPath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            var recordType = typeof(T);
            var properties = recordType.GetProperties();

            csv.WriteRecords(data);

            AnsiConsole.MarkupLine($"[bold green1]=> Data exported successfully to:[/]\n\n[bold dodgerblue1]{Path.GetDirectoryName(fullPath)}\\[/][bold red]{Path.GetFileName(fullPath)}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Error exporting data: {ex.Message}[/]");
        }
    }

    public static void ExportToXlsx<T>(List<T> data)
    {
        try
        {
            string currentDateTime = DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
            string fileName = $"DataManager_{currentDateTime}.xlsx";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(desktopPath, "DataManager", fileName);

            string directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Data");

            var recordType = typeof(T);
            var properties = recordType.GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
            }

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    var property = properties[j];
                    var value = property.GetValue(data[i]);

                    if (property.PropertyType == typeof(Exit))
                    {
                        var exitValue = (Exit)value;
                        worksheet.Cell(i + 2, j + 1).Value = exitValue.Name;
                    }
                    else
                    {
                        if (value != null && (value is int v))
                        {
                            worksheet.Cell(i + 2, j + 1).SetValue(v);
                        }
                        else if (value != null && (value is double v1))
                        {
                            worksheet.Cell(i + 2, j + 1).SetValue(v1);
                        }
                        else if (value != null && (value is decimal v2))
                        {
                            worksheet.Cell(i + 2, j + 1).SetValue(v2);
                        }
                        else
                        {
                            worksheet.Cell(i + 2, j + 1).Value = value != null ? value.ToString() : string.Empty;
                        }
                    }
                }
            }

            workbook.SaveAs(fullPath);

            AnsiConsole.MarkupLine($"[bold green1]=> Data exported successfully to:[/]\n\n[bold dodgerblue1]{Path.GetDirectoryName(fullPath)}\\[/][bold red]{Path.GetFileName(fullPath)}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[bold red]Error exporting data: {ex.Message}[/]");
        }
    }

    public void DatabaseOverview()
    {
        if (!_dbContext.Database.CanConnect())
        {
            AnsiConsole.MarkupLine("[bold red]DataManager database doesn't exist![/]");
            return;
        }

        if (!_dbContext.DataModelOnes.Any() || !_dbContext.DataModelTwos.Any() || !_dbContext.Exits.Any())
        {
            AnsiConsole.MarkupLine("[bold red]A table in DataManager database contains no data![/]");
            return;
        }

        var table = new Table();
        table.AddColumn("[bold dodgerblue1]DataManager Table[/]");
        table.AddColumn("[bold red]Total rows[/]");
        table.Columns[0].Centered();
        table.Columns[1].Centered();
        table.Centered();
        table.Border(TableBorder.Rounded);

        int dataModelTwosRowCount = _dbContext.DataModelTwos.Count();
        int dataModelOnesRowCount = _dbContext.DataModelOnes.Count();
        int exitRowCount = _dbContext.Exits.Count();

        table.AddRow("[bold dodgerblue1]DataModelTwos[/]", $"[bold red]{dataModelTwosRowCount}[/]");
        table.AddRow("[bold dodgerblue1]DataModelOnes[/]", $"[bold red]{dataModelOnesRowCount}[/]");
        table.AddRow("[bold dodgerblue1]Exits[/]", $"[bold red]{exitRowCount}[/]");

        AnsiConsole.Write(table);

        var duplicateExits = _dbContext.Exits.GroupBy(e => e.Name)
                                            .Where(g => g.Count() > 1)
                                            .Select(g => new { ExitId = g.Key, Count = g.Count() })
                                            .ToList();

        if (duplicateExits.Any())
        {
            AnsiConsole.MarkupLine($"\n[bold red]=> There are {duplicateExits.Count} duplicate exits in the database![/]");
        }
    }
}
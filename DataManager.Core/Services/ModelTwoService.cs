using Spectre.Console;
using ClosedXML.Excel;
using System.Globalization;
using DataManager.Core.DBModels;
using DataManager.Core.DbValidation;

namespace DataManager.Core.Services;

public class ModelTwoService
{
    public static List<ModelTwo> ParseXlsx(string filePath)
    {
        AnsiConsole.MarkupLine($"[bold dodgerblue1]Parsing data from:[/] [bold red]{Path.GetFileName(filePath)}[/]");

        List<ModelTwo> modelTwoList = [];
        int parsedRows = 0;

        using (var workbook = new XLWorkbook(filePath))
        {
            IXLWorksheet worksheet = workbook.Worksheet(1);

            if (worksheet != null)
            {
                foreach (IXLRow row in worksheet.Rows().Skip(2))
                {
                    ModelTwo modelTwoData = new();

                    try
                    {
                        modelTwoData.PeriodStartDate = ParseDateOnly(row.Cell(1).GetString());
                        modelTwoData.Exit = new Exit { Name = row.Cell(2).GetValue<string>() };
                        modelTwoData.GainAmountThree = row.Cell(3).GetValue<int>();
                        modelTwoData.PeriodEndDate = ParseDateTime(row.Cell(4).GetString());

                        modelTwoList.Add(modelTwoData);
                        parsedRows++;
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[bold red]Error processing row {row.RowNumber()}: {ex.Message}[/]");
                        modelTwoList.Clear();
                        return modelTwoList;
                    }
                }

                AnsiConsole.MarkupLine($"[bold dodgerblue1]=> Parsed[/] [bold red]{parsedRows}[/] [bold dodgerblue1]rows![/]\n");
            }
        }
        return modelTwoList;
    }

    private static DateOnly ParseDateOnly(string dateString)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            throw new ArgumentException("Date string cannot be null or empty.", nameof(dateString));
        }

        if (DateTime.TryParseExact(dateString.TrimEnd('.'), "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
        {
            return new DateOnly(result.Year, result.Month, result.Day);
        }
        else
        {
            throw new FormatException("Invalid date format!");
        }
    }

    private static DateTime ParseDateTime(string dateTimeString)
    {
        if (string.IsNullOrEmpty(dateTimeString))
        {
            throw new ArgumentNullException(nameof(dateTimeString), "DateTime string cannot be null or empty.");
        }

        if (DateTime.TryParseExact(dateTimeString, "dd.MM.yyyy. HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
        {
            return DateTime.SpecifyKind(parsedDateTime, DateTimeKind.Utc);
        }
        else
        {
            throw new FormatException("Invalid date-time format! Expected format: dd.MM.yyyy. HH:mm:ss");
        }
    }

    public static void ImportModelTwos(DataManagerDbContext dbContext, List<ModelTwo> modelTwos, Dictionary<string, Exit> exitByName)
    {
        var modelTwoValidator = new ModelTwoValidator();

        foreach (var modelTwo in modelTwos)
        {
            var validationResult = modelTwoValidator.Validate(modelTwo);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[bold red]=> Validation error for ModelTwo: {error.ErrorMessage}[/]");
                }
                continue;
            }

            modelTwo.Exit = exitByName[modelTwo.Exit.Name];
            dbContext.ModelTwos.Add(modelTwo);
        }
    }

    public static List<ModelTwo> FetchModelTwoData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        IQueryable<ModelTwo> modelTwoQuery = context.ModelTwos.Where(f => f.PeriodStartDate >= dateFrom);

        if (dateTo != default)
        {
            modelTwoQuery = modelTwoQuery.Where(f => f.PeriodStartDate <= dateTo);
        }

        if (exitId != default)
        {
            modelTwoQuery = modelTwoQuery.Where(f => f.ExitId == exitId);
        }

        List<ModelTwo> dataModelTwos = [.. modelTwoQuery.OrderBy(f => f.PeriodStartDate).ThenBy(f => f.ExitId)];

        return dataModelTwos;
    }
}
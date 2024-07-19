using Spectre.Console;
using ClosedXML.Excel;
using System.Globalization;
using DataManager.Core.DBModels;
using DataManager.Core.DbValidation;

namespace DataManager.Core.Services;

public class DataModelTwoService
{
    public static List<DataModelTwo> ParseXlsx(string filePath)
    {
        AnsiConsole.MarkupLine($"[bold dodgerblue1]Parsing data from:[/] [bold red]{Path.GetFileName(filePath)}[/]");

        List<DataModelTwo> dataModelTwoList = [];
        int parsedRows = 0;

        using (var workbook = new XLWorkbook(filePath))
        {
            IXLWorksheet worksheet = workbook.Worksheet(1);

            if (worksheet != null)
            {
                foreach (IXLRow row in worksheet.Rows().Skip(2))
                {
                    DataModelTwo dataModelTwoData = new();

                    try
                    {
                        dataModelTwoData.PeriodStartDate = ParseDateOnly(row.Cell(1).GetString());
                        dataModelTwoData.Exit = new Exit { Name = row.Cell(2).GetValue<string>() };
                        dataModelTwoData.GainAmountThree = row.Cell(3).GetValue<int>();
                        dataModelTwoData.PeriodEndDate = ParseDateTime(row.Cell(4).GetString());

                        dataModelTwoList.Add(dataModelTwoData);
                        parsedRows++;
                    }
                    catch (Exception ex)
                    {
                        AnsiConsole.MarkupLine($"[bold red]Error processing row {row.RowNumber()}: {ex.Message}[/]");
                        dataModelTwoList.Clear();
                        return dataModelTwoList;
                    }
                }

                AnsiConsole.MarkupLine($"[bold dodgerblue1]=> Parsed[/] [bold red]{parsedRows}[/] [bold dodgerblue1]rows![/]\n");
            }
        }
        return dataModelTwoList;
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

    public static void ImportDataModelTwos(DataManagerDbContext dbContext, List<DataModelTwo> dataModelTwos, Dictionary<string, Exit> exitByName)
    {
        var dataModelTwoValidator = new DataModelTwoValidator();

        foreach (var dataModelTwo in dataModelTwos)
        {
            var validationResult = dataModelTwoValidator.Validate(dataModelTwo);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[bold red]=> Validation error for DataModelTwo: {error.ErrorMessage}[/]");
                }
                continue;
            }

            dataModelTwo.Exit = exitByName[dataModelTwo.Exit.Name];
            dbContext.DataModelTwos.Add(dataModelTwo);
        }
    }

    public static List<DataModelTwo> FetchDataModelTwoData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        IQueryable<DataModelTwo> dataModelTwoQuery = context.DataModelTwos.Where(f => f.PeriodStartDate >= dateFrom);

        if (dateTo != default)
        {
            dataModelTwoQuery = dataModelTwoQuery.Where(f => f.PeriodStartDate <= dateTo);
        }

        if (exitId != default)
        {
            dataModelTwoQuery = dataModelTwoQuery.Where(f => f.ExitId == exitId);
        }

        List<DataModelTwo> dataModelTwos = [.. dataModelTwoQuery.OrderBy(f => f.PeriodStartDate).ThenBy(f => f.ExitId)];

        return dataModelTwos;
    }
}
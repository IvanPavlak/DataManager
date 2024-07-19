using CsvHelper;
using Spectre.Console;
using System.Globalization;
using CsvHelper.Configuration;
using DataManager.Core.DBModels;
using DataManager.Core.DbValidation;
using FluentValidation;

namespace DataManager.Core.Services;

public class DataModelOneService
{
    public static List<DataModelOne> ParseCsv(string filePath)
    {
        AnsiConsole.MarkupLine($"[bold dodgerblue1]Parsing data from:[/] [bold red]{Path.GetFileName(filePath)}[/]");

        List<DataModelOne> dataModelOneList = [];
        int parsedRows = 0;

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            try
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    DataModelOne dataModelOneData = new()
                    {
                        Exit = new Exit { Name = csv.GetField<string>("Exit") },
                        Port = csv.GetField<string>("Port"),
                        UserGroup = csv.GetField<string>("User Group"),
                        Country = csv.GetField<string>("Country"),
                        MemberId = csv.GetField<int>("Member ID"),
                        Date = DateOnly.FromDateTime(csv.GetField<DateTime>("Date")),
                        GainAmountOne = csv.GetField<int>("Gain Amount One"),
                        GainAmountTwo = csv.GetField<int>("Gain Amount Two"),
                        Loss = csv.GetField<int>("Loss Amount"),
                        Total = csv.GetField<int>("Total Amount")
                    };

                    dataModelOneList.Add(dataModelOneData);
                    parsedRows++;
                }

                AnsiConsole.MarkupLine($"[bold dodgerblue1]=> Parsed[/] [bold red]{parsedRows}[/] [bold dodgerblue1]rows![/]\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing: {ex.Message}");
                dataModelOneList.Clear();
                return dataModelOneList;
            }
        }
        return dataModelOneList;
    }

    public static void ImportDataModelOnes(DataManagerDbContext dbContext, List<DataModelOne> dataModelOnes, Dictionary<string, Exit> exitByName)
    {
        var dataModelOneValidator = new DataModelOneValidator();

        foreach (var dataModelOne in dataModelOnes)
        {
            var validationResult = dataModelOneValidator.Validate(dataModelOne);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[bold red]=> Validation error for DataModelOne: {error.ErrorMessage}[/]");
                }
                continue;
            }

            dataModelOne.Exit = exitByName[dataModelOne.Exit.Name];
            dbContext.DataModelOnes.Add(dataModelOne);
        }
    }

    public static List<DataModelOne> FetchDataModelOneData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        IQueryable<DataModelOne> dataModelOneQuery = context.DataModelOnes.Where(a => a.Date >= dateFrom);

        if (dateTo != default)
        {
            dataModelOneQuery = dataModelOneQuery.Where(a => a.Date <= dateTo);
        }

        if (exitId != default)
        {
            dataModelOneQuery = dataModelOneQuery.Where(a => a.ExitId == exitId);
        }

        List<DataModelOne> dataModelOnes;

        if (exitId == default)
        {
            dataModelOnes = [.. dataModelOneQuery.OrderBy(a => a.Date)];
        }
        else
        {
            dataModelOnes = [.. dataModelOneQuery.OrderBy(a => a.ExitId).ThenBy(a => a.Date)];
        }

        return dataModelOnes;
    }
}
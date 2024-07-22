using CsvHelper;
using Spectre.Console;
using System.Globalization;
using CsvHelper.Configuration;
using DataManager.Core.DBModels;
using DataManager.Core.DbValidation;
using FluentValidation;

namespace DataManager.Core.Services;

public class ModelOneService
{
    public static List<ModelOne> ParseCsv(string filePath)
    {
        AnsiConsole.MarkupLine($"[bold dodgerblue1]Parsing data from:[/] [bold red]{Path.GetFileName(filePath)}[/]");

        List<ModelOne> modelOneList = [];
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
                    ModelOne modelOneData = new()
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

                    modelOneList.Add(modelOneData);
                    parsedRows++;
                }

                AnsiConsole.MarkupLine($"[bold dodgerblue1]=> Parsed[/] [bold red]{parsedRows}[/] [bold dodgerblue1]rows![/]\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing: {ex.Message}");
                modelOneList.Clear();
                return modelOneList;
            }
        }
        return modelOneList;
    }

    public static void ImportModelOnes(DataManagerDbContext dbContext, List<ModelOne> modelOnes, Dictionary<string, Exit> exitByName)
    {
        var modelOneValidator = new ModelOneValidator();

        foreach (var modelOne in modelOnes)
        {
            var validationResult = modelOneValidator.Validate(modelOne);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    AnsiConsole.MarkupLine($"[bold red]=> Validation error for ModelOne: {error.ErrorMessage}[/]");
                }
                continue;
            }

            modelOne.Exit = exitByName[modelOne.Exit.Name];
            dbContext.ModelOnes.Add(modelOne);
        }
    }

    public static List<ModelOne> FetchModelOneData(DataManagerDbContext context, DateOnly dateFrom, DateOnly dateTo, int exitId)
    {
        IQueryable<ModelOne> modelOneQuery = context.ModelOnes.Where(a => a.Date >= dateFrom);

        if (dateTo != default)
        {
            modelOneQuery = modelOneQuery.Where(a => a.Date <= dateTo);
        }

        if (exitId != default)
        {
            modelOneQuery = modelOneQuery.Where(a => a.ExitId == exitId);
        }

        List<ModelOne> modelOnes;

        if (exitId == default)
        {
            modelOnes = [.. modelOneQuery.OrderBy(a => a.Date)];
        }
        else
        {
            modelOnes = [.. modelOneQuery.OrderBy(a => a.ExitId).ThenBy(a => a.Date)];
        }

        return modelOnes;
    }
}
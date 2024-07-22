using Spectre.Console;
using DataManager.Core;
using System.Globalization;
using DataManager.Core.Services;
using static DataManager.Core.Services.DataManagerService;

namespace DataManager.Console;

public class DataManagerPrompts
{
    public static void ConsoleAppStartPrompt(DataManagerService DataManagerService, string csvFilePath, string xlsxFilePath, DataManagerDbContext dbContext)
    {
        EraseExistingDatabasePrompt(dbContext);

        var importChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold dodgerblue1]=> Do you want to import data?[/]")
                .PageSize(3)
                .AddChoices("[bold green1]Yes[/]", "[bold red]No[/]"));

        if (importChoice == "[bold green1]Yes[/]")
        {
            AnsiConsole.MarkupLine("[bold green1]=> Importing data![/]");
            PrettifyConsole.CreateAndDisplayLine("dodgerblue1");
            DataManagerService.ImportToDatabase(csvFilePath, DataTypes.ModelOne);
            PrettifyConsole.CreateAndDisplayLine("dodgerblue1");
            DataManagerService.ImportToDatabase(xlsxFilePath, DataTypes.ModelTwo);
            PrettifyConsole.Title("Database Overview", "Up");
            DataManagerService.DatabaseOverview();
            PrettifyConsole.CreateAndDisplayLine("dodgerblue1");
            FetchDataPrompt(dbContext);
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]=> Import canceled![/]");
            PrettifyConsole.Title("Database Overview", "up");
            DataManagerService.DatabaseOverview();
            PrettifyConsole.CreateAndDisplayLine("dodgerblue1");
            FetchDataPrompt(dbContext);
        }
    }

    public static void EraseExistingDatabasePrompt(DataManagerDbContext dbContext)
    {
        var importChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold dodgerblue1]=> Do you want to erase the existing database?[/]")
                .PageSize(3)
                .AddChoices("[bold green1]Yes[/]", "[bold red]No[/]"));

        if (importChoice == "[bold green1]Yes[/]")
        {
            AnsiConsole.MarkupLine($"[bold red]=> DELETED EXISTING DATABASE![/]");
            PrettifyConsole.CreateAndDisplayLine("dodgerblue1");
            dbContext.Database.EnsureDeleted();
        }
        else
        {
            return;
        }
    }

    public static void FetchDataPrompt(DataManagerDbContext dbContext)
    {
        if (!dbContext.Database.CanConnect())
        {
            return;
        }

        if (!dbContext.ModelOnes.Any() || !dbContext.ModelTwos.Any() || !dbContext.Exits.Any())
        {
            return;
        }

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold dodgerblue1]=> Do you want to fetch data?[/]")
                    .PageSize(4)
                    .AddChoices("[bold green1]ModelOne Data[/]",
                                "[bold green1]ModelTwo Data[/]",
                                "[bold green1]Combined Data[/]",
                                "[bold red]Cancel[/]"));

            switch (choice)
            {
                case "[bold green1]ModelOne Data[/]":
                    HandleUserInputAndProcessData(dbContext, DataTypes.ModelOne);
                    break;
                case "[bold green1]ModelTwo Data[/]":
                    HandleUserInputAndProcessData(dbContext, DataTypes.ModelTwo);
                    break;
                case "[bold green1]Combined Data[/]":
                    HandleUserInputAndProcessData(dbContext, DataTypes.CombinedData);
                    break;
                default:
                    AnsiConsole.MarkupLine("[bold red]=> Data fetch canceled![/]");
                    PrettifyConsole.CreateAndDisplayLine("dodgerblue1");
                    return;
            }

            var continueChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold dodgerblue1]=> Do you want to fetch data again?[/]")
                    .PageSize(3)
                    .AddChoices("[bold green1]Yes[/]", "[bold red]No[/]"));

            if (continueChoice == "[bold red]No[/]")
            {
                AnsiConsole.MarkupLine("[bold green1]=> Data fetch completed![/]");
                PrettifyConsole.CreateAndDisplayLine("dodgerblue1");
                break;
            }
        }
    }

    private static void HandleUserInputAndProcessData(DataManagerDbContext dbContext, DataTypes dataType)
    {
        var dateFrom = PromptForDate("dateFrom");
        var dateTo = PromptForDate("dateTo");
        int exitId = PromptForExit(dbContext);

        if (dataType == DataTypes.ModelOne)
        {
            DataManagerDisplay.DisplayModelOneData(dbContext, dateFrom, dateTo, exitId);
            ExportDataPrompt(ModelOneService.FetchModelOneData(dbContext, dateFrom, dateTo, exitId));
        }
        else if (dataType == DataTypes.ModelTwo)
        {
            DataManagerDisplay.DisplayModelTwoData(dbContext, dateFrom, dateTo, exitId);
            ExportDataPrompt(ModelTwoService.FetchModelTwoData(dbContext, dateFrom, dateTo, exitId));
        }
        else if (dataType == DataTypes.CombinedData)
        {
            DataManagerDisplay.DisplayCombinedData(dbContext, dateFrom, dateTo, exitId);
            ExportDataPrompt(CombinedDataService.FetchCombinedData(dbContext, dateFrom, dateTo, exitId));
        }
    }

    private static DateOnly PromptForDate(string date)
    {
        string promptMessage = date.Equals("dateFrom")
          ? "[bold dodgerblue1]Date from ([/][bold red]Required! Format: dd-MM-yyyy[/][bold dodgerblue1]):[/]"
          : "[bold dodgerblue1]Date to ([/][bold green1]Optional![/][bold red] Format: dd-MM-yyyy[/][bold dodgerblue1]):[/]";

        var inputDateString = AnsiConsole.Prompt(new TextPrompt<string>(promptMessage)
            .AllowEmpty()
            .Validate(input =>
            {
                if (date.Equals("dateFrom") && string.IsNullOrEmpty(input))
                {
                    return ValidationResult.Error("\n[bold red]This is a required field![/]\n");
                }
                if (date.Equals("dateTo") && string.IsNullOrEmpty(input))
                {
                    AnsiConsole.MarkupLine("\n[bold green1]=> Defaulting to latest known date![/]");
                    return ValidationResult.Success();
                }
                else if (!DateOnly.TryParseExact(input, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                {
                    return ValidationResult.Error("\n[bold red]Invalid format! Example input: 01-01-2024[/]\n");
                }

                return ValidationResult.Success();
            }));

        return DateOnly.TryParseExact(inputDateString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly inputDate)
            ? new DateOnly(inputDate.Year, inputDate.Month, inputDate.Day)
            : default;
    }

    private static int PromptForExit(DataManagerDbContext dbContext)
    {
        var specifiedExit = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("\n[bold dodgerblue1]Do you want to specify the Exit?[/]")
                .PageSize(3)
                .AddChoices("[bold green1]Yes[/]", "[bold red]No[/]"));

        if (specifiedExit == "[bold green1]Yes[/]")
        {
            return SelectExit(dbContext);
        }
        return 0;
    }

    private static int SelectExit(DataManagerDbContext dbContext)
    {
        using var context = new DataManagerDbContext();
        var exits = context.Exits.OrderBy(e => e.Name).ToList();

        if (exits.Count != 0)
        {
            var exitOptions = exits.Select(exit => $"{exit.Id}: {exit.Name}").ToList();

            var selectedExit = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold dodgerblue1]\nSelect an [bold red]Exit[/]:[/]")
                    .PageSize(10)
                    .AddChoices(exitOptions));

            return int.Parse(selectedExit.Split(':')[0]);
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]Fetching data for all exits.[/]");
            return 0;
        }
    }

    public static void ExportDataPrompt<T>(List<T> data)
    {
        PrettifyConsole.CreateAndDisplayLine("dodgerblue1");

        var exportChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold dodgerblue1]=> Do you want to export this data?[/]")
                        .PageSize(3)
                        .AddChoices("[bold green1]Yes[/]", "[bold red]No[/]"));

        if (exportChoice == "[bold green1]Yes[/]")
        {
            var exportFormatChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold dodgerblue1]Select export format:[/]")
                    .PageSize(3)
                    .AddChoices("[bold green1]CSV[/]", "[bold green1]XLSX[/]", "[bold red]Cancel[/]"));

            switch (exportFormatChoice)
            {
                case "[bold green1]CSV[/]":
                    ExportToCsv(data);
                    break;
                case "[bold green1]XLSX[/]":
                    ExportToXlsx(data);
                    break;
                case "[bold red]Cancel[/]":
                    AnsiConsole.MarkupLine("[bold red]=> Data export canceled![/]");
                    break;
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[bold red]=> Data export canceled![/]");
        }

        PrettifyConsole.CreateAndDisplayLine("dodgerblue1");
    }

    public static string PaginationPrompt()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<string>("[bold dodgerblue1]Press[/] [bold green1]n[/] [bold dodgerblue1]for[/] [bold green1]next page[/][bold dodgerblue1],[/] [bold yellow]p[/] [bold dodgerblue1]for[/] [bold yellow]previous page[/][bold dodgerblue1], or[/] [bold red]e[/] [bold dodgerblue1]to[/] [bold red]exit[/][bold dodgerblue1]![/]")
                .AllowEmpty()
                .Validate(input =>
                {
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        return ValidationResult.Error("[bold red]Invalid choice. Please enter [bold green1]n[/], [bold yellow]p[/], or [bold red]e[/]![/]");
                    }

                    var choice = input.ToLower();
                    if (choice != "n" && choice != "p" && choice != "e")
                    {
                        return ValidationResult.Error("[bold red]Invalid choice. Please enter [bold green1]n[/], [bold yellow]p[/], or [bold red]e[/]![/]");
                    }

                    return ValidationResult.Success();
                }));
    }
}
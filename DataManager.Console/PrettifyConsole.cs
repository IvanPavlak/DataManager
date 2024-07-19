using DataManager.Core;
using Spectre.Console;

namespace DataManager.Console;

public class PrettifyConsole
{
    public static void CreateAndDisplayLine(string color)
    {
        var rule = new Rule();
        rule.RuleStyle(color);
        AnsiConsole.Write(rule);
    }

    public static void ProgressBar(string taskDescription)
    {
        AnsiConsole.Progress()
            .Columns(
            [
                new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new SpinnerColumn(Spinner.Known.Dots)
            ])
            .Start(ctx =>
            {
                var task = ctx.AddTask(taskDescription);

                while (!ctx.IsFinished)
                {
                    task.Increment(5);
                    Thread.Sleep(50);
                }
            });
    }

    public static void Title(string title, string border = "both")
    {
        if (border.Equals("up", StringComparison.OrdinalIgnoreCase) || border.Equals("both"))
        {
            CreateAndDisplayLine("dodgerblue1");
        }

        AnsiConsole.Write(
            new FigletText(title)
                .Centered()
                .Color(Color.DodgerBlue1)
        );

        if (border.Equals("down", StringComparison.OrdinalIgnoreCase) || border.Equals("both"))
        {
            CreateAndDisplayLine("dodgerblue1");
        }
    }

    public static void Pagination<T>(List<T> data, DataManagerDbContext dbContext, int pageNumber, int pageSize, Action<List<T>, DataManagerDbContext, int, int> displayMethod)
    {
        var totalItems = data.Count;
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        AnsiConsole.MarkupLine($"[bold dodgerblue1]Page:[/] [bold green1]{pageNumber}[/][bold dodgerblue1]/[/][bold red]{totalPages}[/]");
        AnsiConsole.MarkupLine($"[bold dodgerblue1]Total Items:[/] [bold red]{totalItems}[/]");

        if (totalPages > 1)
        {
            bool exitLoop = false;

            while (!exitLoop)
            {
                var choice = DataManagerPrompts.PaginationPrompt();
                if (choice == "n")
                {
                    if (pageNumber < totalPages)
                    {
                        displayMethod(data, dbContext, pageNumber + 1, pageSize);
                        exitLoop = true;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[bold red]You are on the last page![/]");
                    }
                }
                else if (choice == "p")
                {
                    if (pageNumber > 1)
                    {
                        displayMethod(data, dbContext, pageNumber - 1, pageSize);
                        exitLoop = true;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[bold red]You are on the first page![/]");
                    }
                }
                else if (choice == "e")
                {
                    AnsiConsole.MarkupLine($"\n[bold red]=> Exiting![/]");
                    CreateAndDisplayLine("dodgerblue1");
                    exitLoop = true;
                }
            }
        }
    }
}
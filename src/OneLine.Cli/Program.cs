using System.CommandLine;
using OneLine.Cli.Commands;
using Spectre.Console;

AnsiConsole.Write(
    new FigletText("OneLine CLI")
        .Centered()
        .Color(Color.Blue));

AnsiConsole.MarkupLine("[grey]Un backend SaaS complet en quelques minutes[/]\n");

var rootCommand = new RootCommand("OneLine SaaS Starter Kit CLI");

rootCommand.AddCommand(NewCommand.Create());
rootCommand.AddCommand(AddCommand.Create());

return await rootCommand.InvokeAsync(args);

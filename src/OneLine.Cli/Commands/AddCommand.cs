using System.CommandLine;
using OneLine.Cli.Generators;
using Spectre.Console;

namespace OneLine.Cli.Commands;

/// <summary>
/// Commande : saas add <module>
///
/// Installe un module OneLine dans le projet courant.
/// Pattern : Command Pattern
///
/// Modules disponibles :
///   auth, tenant, billing, security, logging, ai
/// </summary>
public static class AddCommand
{
    public static Command Create()
    {
        var moduleArg = new Argument<string>(
            name: "module",
            description: "Module a installer : auth, tenant, billing, security, logging, ai");

        var pathOption = new Option<string>(
            aliases: ["--path", "-p"],
            description: "Chemin du projet",
            getDefaultValue: () => Directory.GetCurrentDirectory());

        var command = new Command(
            name: "add",
            description: "Installe un module OneLine dans votre projet")
        {
            moduleArg,
            pathOption
        };

        command.SetHandler(async (module, path) =>
        {
            if (!ModuleInstaller.ModuleExists(module))
            {
                AnsiConsole.MarkupLine($"[red]Module inconnu :[/] {module}");
                AnsiConsole.MarkupLine("\n[bold]Modules disponibles :[/]");

                var table = new Table();
                table.AddColumn("Module");
                table.AddColumn("Description");
                table.AddRow("auth",     "Authentication JWT + RBAC");
                table.AddRow("tenant",   "Multi-tenancy");
                table.AddRow("billing",  "Paiement Stripe");
                table.AddRow("security", "Rate limiting + protection");
                table.AddRow("logging",  "Serilog + Prometheus");
                table.AddRow("ai",       "Integration IA (LLM, RAG)");
                AnsiConsole.Write(table);
                return;
            }

            var success = await ModuleInstaller.InstallAsync(module, path);

            if (success)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[green]Module[/] [bold]{module}[/] [green]installe avec succes ![/]");
            }
        },
        moduleArg, pathOption);

        return command;
    }
}

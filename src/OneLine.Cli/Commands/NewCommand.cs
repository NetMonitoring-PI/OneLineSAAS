using System.CommandLine;
using OneLine.Cli.Generators;
using Spectre.Console;

namespace OneLine.Cli.Commands;

public static class NewCommand
{
    public static Command Create()
    {
        var nameArg = new Argument<string>(
            name: "name",
            description: "Nom de l application SaaS a creer");

        var outputOption = new Option<string>(
            aliases: ["--output", "-o"],
            description: "Dossier de sortie",
            getDefaultValue: () => Directory.GetCurrentDirectory());

        var command = new Command(
            name: "new",
            description: "Genere une nouvelle solution SaaS en Clean Architecture")
        {
            nameArg,
            outputOption
        };

        command.SetHandler(async (name, output) =>
        {
            if (string.IsNullOrWhiteSpace(name) ||
                name.Any(c => !char.IsLetterOrDigit(c) && c != '.' && c != '-' && c != '_'))
            {
                AnsiConsole.MarkupLine("[red]Nom invalide.[/] Utilisez uniquement lettres, chiffres, points, tirets.");
                return;
            }

            AnsiConsole.Write(new Rule("[blue]OneLine SaaS Starter Kit[/]"));
            AnsiConsole.MarkupLine($"Creation de [bold]{name}[/] dans [grey]{output}[/]\n");

            var success = await SolutionGenerator.GenerateAsync(name, output);

            if (success)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Rule("[green]Solution creee avec succes ![/]"));
                AnsiConsole.WriteLine();

                var table = new Table();
                table.AddColumn("Commande");
                table.AddColumn("Description");
                table.AddRow($"[blue]cd {name}[/]", "Naviguer dans le projet");
                table.AddRow($"[blue]dotnet build[/]", "Compiler la solution");
                table.AddRow($"[blue]saas add auth[/]", "Ajouter l authentification");
                table.AddRow($"[blue]saas add tenant[/]", "Ajouter le multi-tenancy");
                table.AddRow($"[blue]saas add billing[/]", "Ajouter le paiement Stripe");
                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine("\n[red]Echec de la creation.[/] Verifiez que dotnet est installe.");
            }
        },
        nameArg, outputOption);

        return command;
    }
}

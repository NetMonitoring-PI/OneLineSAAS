using Spectre.Console;
using System.Diagnostics;

namespace OneLine.Cli.Generators;

/// <summary>
/// Genere la structure complete d un projet SaaS en Clean Architecture.
///
/// Structure generee :
///   AppName.Domain/           <- entites, interfaces
///   AppName.Application/      <- use cases, DTOs
///   AppName.Infrastructure/   <- EF Core, services
///   AppName.API/              <- controllers, Program.cs
///   AppName.sln
/// </summary>
public static class SolutionGenerator
{
    public static async Task<bool> GenerateAsync(
        string appName, string outputPath)
    {
        var fullPath = Path.Combine(outputPath, appName);

        AnsiConsole.MarkupLine($"[blue]Creation de la solution[/] [bold]{appName}[/]...");

        // Creer le dossier racine
        Directory.CreateDirectory(fullPath);

        var steps = new List<(string Description, Func<Task<bool>> Action)>
        {
            ("Creation de la solution .NET",
                () => RunDotnetAsync($"new sln -n {appName}", fullPath)),

            ("Creation AppName.Domain",
                () => RunDotnetAsync(
                    $"new classlib -n {appName}.Domain -o src/{appName}.Domain",
                    fullPath)),

            ("Creation AppName.Application",
                () => RunDotnetAsync(
                    $"new classlib -n {appName}.Application -o src/{appName}.Application",
                    fullPath)),

            ("Creation AppName.Infrastructure",
                () => RunDotnetAsync(
                    $"new classlib -n {appName}.Infrastructure -o src/{appName}.Infrastructure",
                    fullPath)),

            ("Creation AppName.API",
                () => RunDotnetAsync(
                    $"new webapi -n {appName}.API -o src/{appName}.API --no-openapi",
                    fullPath)),

            ("Creation projet de Tests",
                () => RunDotnetAsync(
                    $"new xunit -n {appName}.Tests -o tests/{appName}.Tests",
                    fullPath)),

            ($"Ajout {appName}.Domain a la solution",
                () => RunDotnetAsync(
                    $"sln add src/{appName}.Domain/{appName}.Domain.csproj",
                    fullPath)),

            ($"Ajout {appName}.Application a la solution",
                () => RunDotnetAsync(
                    $"sln add src/{appName}.Application/{appName}.Application.csproj",
                    fullPath)),

            ($"Ajout {appName}.Infrastructure a la solution",
                () => RunDotnetAsync(
                    $"sln add src/{appName}.Infrastructure/{appName}.Infrastructure.csproj",
                    fullPath)),

            ($"Ajout {appName}.API a la solution",
                () => RunDotnetAsync(
                    $"sln add src/{appName}.API/{appName}.API.csproj",
                    fullPath)),

            ($"Ajout {appName}.Tests a la solution",
                () => RunDotnetAsync(
                    $"sln add tests/{appName}.Tests/{appName}.Tests.csproj",
                    fullPath)),

            ("Reference Application -> Domain",
                () => RunDotnetAsync(
                    $"add src/{appName}.Application/{appName}.Application.csproj " +
                    $"reference src/{appName}.Domain/{appName}.Domain.csproj",
                    fullPath)),

            ("Reference Infrastructure -> Application",
                () => RunDotnetAsync(
                    $"add src/{appName}.Infrastructure/{appName}.Infrastructure.csproj " +
                    $"reference src/{appName}.Application/{appName}.Application.csproj",
                    fullPath)),

            ("Reference API -> Infrastructure",
                () => RunDotnetAsync(
                    $"add src/{appName}.API/{appName}.API.csproj " +
                    $"reference src/{appName}.Infrastructure/{appName}.Infrastructure.csproj",
                    fullPath)),
        };

        foreach (var (desc, action) in steps)
        {
            var ok = await AnsiConsole.Status()
                .StartAsync(desc, async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    return await action();
                });

            if (!ok)
            {
                AnsiConsole.MarkupLine($"[red]Echec :[/] {desc}");
                return false;
            }
        }

        // Generer les fichiers de base
        await GenerateBaseFilesAsync(appName, fullPath);

        return true;
    }

    private static async Task GenerateBaseFilesAsync(
        string appName, string basePath)
    {
        // .gitignore
        await RunDotnetAsync("new gitignore", basePath);

        // global.json
        var globalJson = """
            {
              "sdk": {
                "version": "9.0.0",
                "rollForward": "latestMinor"
              }
            }
            """;
        await File.WriteAllTextAsync(
            Path.Combine(basePath, "global.json"), globalJson);

        // README.md
        var readme = $"""
            # {appName}

            Backend SaaS genere par [OneLine CLI](https://github.com/your-repo/OneLine.SaasKit)

            ## Architecture

            ```
            src/
            â”œâ”€â”€ {appName}.Domain/          <- Entites, interfaces (0 dependance)
            â”œâ”€â”€ {appName}.Application/     <- Use cases, DTOs, CQRS
            â”œâ”€â”€ {appName}.Infrastructure/  <- EF Core, services externes
            â””â”€â”€ {appName}.API/             <- Controllers, middleware, Program.cs
            ```

            ## Demarrage

            ```bash
            dotnet restore
            dotnet build
            dotnet run --project src/{appName}.API
            ```

            ## Modules installes

            Voir les packages NuGet dans chaque .csproj.
            """;
        await File.WriteAllTextAsync(
            Path.Combine(basePath, "README.md"), readme);

        // appsettings.json dans l API
        var apiPath = Path.Combine(basePath, "src", $"{appName}.API");
        if (Directory.Exists(apiPath))
        {
            var appsettings = """
                {
                  "ConnectionStrings": {
                    "DefaultConnection": "Host=localhost;Port=5432;Database=your_db;Username=postgres;Password=postgres"
                  },
                  "Jwt": {
                    "SecretKey": "CHANGE-THIS-SECRET-KEY-MINIMUM-32-CHARS!!",
                    "Issuer": "YourAPI",
                    "Audience": "YourClient",
                    "AccessTokenExpiryMinutes": 15,
                    "RefreshTokenExpiryDays": 7
                  },
                  "Logging": {
                    "LogLevel": {
                      "Default": "Information",
                      "Microsoft.AspNetCore": "Warning"
                    }
                  },
                  "AllowedHosts": "*"
                }
                """;
            await File.WriteAllTextAsync(
                Path.Combine(apiPath, "appsettings.json"), appsettings);
        }
    }

    private static async Task<bool> RunDotnetAsync(
        string arguments, string workingDir)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = arguments,
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi)!;
            await process.WaitForExitAsync();
            return process.ExitCode == 0;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Erreur : {ex.Message}[/]");
            return false;
        }
    }
}

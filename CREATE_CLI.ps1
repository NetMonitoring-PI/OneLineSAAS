# ============================================================
# Script CLI Tool - OneLine.Cli
# Executer depuis : C:\Users\DELL\Projects\OneLine.SaasKit
# ============================================================

Write-Host "=== CLI Tool OneLine ===" -ForegroundColor Cyan

# ── ETAPE 1 : Creer le projet ────────────────────────────────
Write-Host "`n[1/5] Creation du projet CLI..." -ForegroundColor Yellow

dotnet new console -n OneLine.Cli -o src\OneLine.Cli --force
dotnet sln add src\OneLine.Cli\OneLine.Cli.csproj

Write-Host "Projet CLI cree" -ForegroundColor Green

# ── ETAPE 2 : Packages ───────────────────────────────────────
Write-Host "`n[2/5] Installation packages..." -ForegroundColor Yellow

dotnet add src\OneLine.Cli\OneLine.Cli.csproj package System.CommandLine --version 2.0.0-beta4.22272.1
dotnet add src\OneLine.Cli\OneLine.Cli.csproj package Spectre.Console --version 0.49.1

Write-Host "Packages installes" -ForegroundColor Green

# ── ETAPE 3 : Dossiers ───────────────────────────────────────
Write-Host "`n[3/5] Creation des dossiers..." -ForegroundColor Yellow

$dirs = @(
    "src\OneLine.Cli\Commands",
    "src\OneLine.Cli\Generators",
    "src\OneLine.Cli\Templates"
)
foreach ($dir in $dirs) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }

Write-Host "Dossiers crees" -ForegroundColor Green

# ── ETAPE 4 : Fichiers ───────────────────────────────────────
Write-Host "`n[4/5] Creation des fichiers..." -ForegroundColor Yellow

# ── Program.cs ───────────────────────────────────────────────
Set-Content -Path "src\OneLine.Cli\Program.cs" -Encoding UTF8 -Value @'
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
'@

# ── SolutionGenerator.cs ─────────────────────────────────────
Set-Content -Path "src\OneLine.Cli\Generators\SolutionGenerator.cs" -Encoding UTF8 -Value @'
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
            ├── {appName}.Domain/          <- Entites, interfaces (0 dependance)
            ├── {appName}.Application/     <- Use cases, DTOs, CQRS
            ├── {appName}.Infrastructure/  <- EF Core, services externes
            └── {appName}.API/             <- Controllers, middleware, Program.cs
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
'@

# ── ModuleInstaller.cs ────────────────────────────────────────
Set-Content -Path "src\OneLine.Cli\Generators\ModuleInstaller.cs" -Encoding UTF8 -Value @'
using Spectre.Console;
using System.Diagnostics;

namespace OneLine.Cli.Generators;

/// <summary>
/// Installe les modules OneLine dans un projet existant.
/// Pattern : Command + Strategy
///
/// Chaque module correspond a une "saas add <module>" command.
/// L installeur ajoute les packages NuGet et les fichiers necessaires.
/// </summary>
public static class ModuleInstaller
{
    private static readonly Dictionary<string, ModuleDefinition> Modules = new()
    {
        ["auth"] = new ModuleDefinition(
            "Authentication JWT + RBAC",
            new[]
            {
                "Microsoft.AspNetCore.Authentication.JwtBearer",
                "Microsoft.AspNetCore.Identity.EntityFrameworkCore",
                "System.IdentityModel.Tokens.Jwt"
            },
            GenerateAuthInstructions),

        ["tenant"] = new ModuleDefinition(
            "Multi-tenancy (isolation des donnees par client)",
            new[]
            {
                "Microsoft.EntityFrameworkCore",
                "Npgsql.EntityFrameworkCore.PostgreSQL"
            },
            GenerateTenantInstructions),

        ["billing"] = new ModuleDefinition(
            "Paiement Stripe + abonnements + webhooks",
            new[]
            {
                "Stripe.net"
            },
            GenerateBillingInstructions),

        ["security"] = new ModuleDefinition(
            "Rate limiting + protection brute force + API Keys",
            new[]
            {
                "Microsoft.Extensions.Caching.Memory"
            },
            GenerateSecurityInstructions),

        ["logging"] = new ModuleDefinition(
            "Logging structure Serilog + metriques Prometheus",
            new[]
            {
                "Serilog.AspNetCore",
                "Serilog.Sinks.Console",
                "Serilog.Sinks.File",
                "prometheus-net.AspNetCore"
            },
            GenerateLoggingInstructions),

        ["ai"] = new ModuleDefinition(
            "Integration IA (LLM, RAG, quotas par tenant)",
            new[]
            {
                "Microsoft.Extensions.AI",
                "Microsoft.Extensions.AI.OpenAI"
            },
            GenerateAiInstructions),
    };

    public static bool ModuleExists(string moduleName)
        => Modules.ContainsKey(moduleName.ToLowerInvariant());

    public static IEnumerable<string> GetAvailableModules()
        => Modules.Keys;

    public static async Task<bool> InstallAsync(
        string moduleName, string projectPath)
    {
        var key = moduleName.ToLowerInvariant();
        if (!Modules.TryGetValue(key, out var module))
        {
            AnsiConsole.MarkupLine($"[red]Module inconnu : {moduleName}[/]");
            return false;
        }

        AnsiConsole.MarkupLine(
            $"\n[blue]Installation du module[/] [bold]{moduleName}[/]");
        AnsiConsole.MarkupLine($"[grey]{module.Description}[/]\n");

        // Trouver le projet Infrastructure ou API
        var infraProject = FindProject(projectPath, "Infrastructure");
        var apiProject = FindProject(projectPath, "API");

        if (infraProject is null && apiProject is null)
        {
            AnsiConsole.MarkupLine("[red]Aucun projet .csproj trouve.[/]");
            AnsiConsole.MarkupLine("[grey]Assurez-vous d etre dans un dossier de projet OneLine.[/]");
            return false;
        }

        var targetProject = infraProject ?? apiProject!;

        // Installer les packages NuGet
        foreach (var package in module.Packages)
        {
            var ok = await AnsiConsole.Status()
                .StartAsync($"Installation {package}...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Dots);
                    return await RunDotnetAsync(
                        $"add \"{targetProject}\" package {package}",
                        projectPath);
                });

            if (ok)
                AnsiConsole.MarkupLine($"  [green]OK[/] {package}");
            else
                AnsiConsole.MarkupLine($"  [yellow]WARN[/] {package} - verifiez manuellement");
        }

        // Afficher les instructions
        module.PrintInstructions(moduleName);

        return true;
    }

    private static string? FindProject(string basePath, string suffix)
    {
        return Directory
            .GetFiles(basePath, "*.csproj", SearchOption.AllDirectories)
            .FirstOrDefault(f => f.Contains(suffix));
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
        catch { return false; }
    }

    private static void GenerateAuthInstructions(string _)
    {
        var panel = new Panel("""
            [bold]Etapes suivantes :[/]

            1. Ajouter dans Program.cs :
               [blue]builder.Services.AddAuthentication()
               builder.Services.AddAuthorization()[/]

            2. Configurer appsettings.json :
               [blue]"Jwt": {
                 "SecretKey": "votre-cle-secrete-32-chars",
                 "Issuer": "VotreAPI",
                 "Audience": "VotreClient"
               }[/]

            3. Creer AppUser, AuthController, TokenService
               (voir la doc OneLine pour les templates)
            """)
        {
            Header = new PanelHeader("[green] Auth Module installe [/]"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel);
    }

    private static void GenerateTenantInstructions(string _)
    {
        var panel = new Panel("""
            [bold]Etapes suivantes :[/]

            1. Ajouter TenantMiddleware dans Program.cs :
               [blue]app.UseMiddleware<TenantMiddleware>()[/]

            2. Ajouter TenantId sur vos entites :
               [blue]public Guid TenantId { get; set; }[/]

            3. Configurer Global Query Filter dans DbContext :
               [blue]builder.Entity<T>().HasQueryFilter(
                 x => x.TenantId == _currentTenant.TenantId)[/]
            """)
        {
            Header = new PanelHeader("[green] Tenant Module installe [/]"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel);
    }

    private static void GenerateBillingInstructions(string _)
    {
        var panel = new Panel("""
            [bold]Etapes suivantes :[/]

            1. Configurer appsettings.json :
               [blue]"Stripe": {
                 "SecretKey": "sk_test_...",
                 "WebhookSecret": "whsec_..."
               }[/]

            2. Creer les entites : Plan, Subscription, Invoice

            3. Configurer le webhook Stripe :
               Dashboard Stripe -> Webhooks -> votre URL/api/billing/webhook
            """)
        {
            Header = new PanelHeader("[green] Billing Module installe [/]"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel);
    }

    private static void GenerateSecurityInstructions(string _)
    {
        var panel = new Panel("""
            [bold]Etapes suivantes :[/]

            1. Ajouter dans Program.cs :
               [blue]builder.Services.AddMemoryCache()
               app.UseMiddleware<RateLimitMiddleware>()[/]

            2. Configurer appsettings.json :
               [blue]"Security": {
                 "MaxRequestsPerMinutePerIp": 60,
                 "MaxFailedLoginAttempts": 5
               }[/]
            """)
        {
            Header = new PanelHeader("[green] Security Module installe [/]"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel);
    }

    private static void GenerateLoggingInstructions(string _)
    {
        var panel = new Panel("""
            [bold]Etapes suivantes :[/]

            1. Ajouter dans Program.cs :
               [blue]app.UseMiddleware<CorrelationIdMiddleware>()
               app.UseMetricServer()
               app.UseHttpMetrics()[/]

            2. Acceder aux metriques :
               [blue]http://localhost:5000/metrics[/]

            3. Les logs sont dans :
               [blue]logs/oneline-YYYY-MM-DD.log[/]
            """)
        {
            Header = new PanelHeader("[green] Logging Module installe [/]"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel);
    }

    private static void GenerateAiInstructions(string _)
    {
        var panel = new Panel("""
            [bold]Etapes suivantes :[/]

            1. Configurer appsettings.json :
               [blue]"AI": {
                 "Provider": "OpenAI",
                 "ApiKey": "sk-...",
                 "Model": "gpt-4o"
               }[/]

            2. Ajouter dans Program.cs :
               [blue]builder.Services.AddOpenAIClient(...)[/]

            3. Injecter IChatClient dans vos services

            [grey]Module AI en version preview - voir la doc[/]
            """)
        {
            Header = new PanelHeader("[green] AI Module installe [/]"),
            Border = BoxBorder.Rounded
        };
        AnsiConsole.Write(panel);
    }
}

public sealed record ModuleDefinition(
    string Description,
    string[] Packages,
    Action<string> PrintInstructions);
'@

# ── NewCommand.cs ─────────────────────────────────────────────
Set-Content -Path "src\OneLine.Cli\Commands\NewCommand.cs" -Encoding UTF8 -Value @'
using System.CommandLine;
using OneLine.Cli.Generators;
using Spectre.Console;

namespace OneLine.Cli.Commands;

/// <summary>
/// Commande : saas new <AppName>
///
/// Genere une solution complete en Clean Architecture.
/// Pattern : Command Pattern
/// </summary>
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
            // Validation du nom
            if (string.IsNullOrWhiteSpace(name) ||
                name.Any(c => !char.IsLetterOrDigit(c) && c != '.' && c != '-' && c != '_'))
            {
                AnsiConsole.MarkupLine("[red]Nom invalide.[/] Utilisez uniquement lettres, chiffres, points, tirets.");
                return;
            }

            AnsiConsole.Rule("[blue]OneLine SaaS Starter Kit[/]");
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
'@

# ── AddCommand.cs ─────────────────────────────────────────────
Set-Content -Path "src\OneLine.Cli\Commands\AddCommand.cs" -Encoding UTF8 -Value @'
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
'@

Write-Host "Fichiers crees" -ForegroundColor Green

# ── ETAPE 5 : Build ──────────────────────────────────────────
Write-Host "`n[5/5] Build..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -eq 0) {
    Write-Host "`n=== BUILD REUSSI ===" -ForegroundColor Green
    Write-Host "`nTest du CLI :" -ForegroundColor Cyan
    Write-Host "dotnet run --project src\OneLine.Cli\OneLine.Cli.csproj -- new MonApp" -ForegroundColor Gray
    Write-Host "dotnet run --project src\OneLine.Cli\OneLine.Cli.csproj -- add auth" -ForegroundColor Gray
    Write-Host "`nOu publier comme outil global :" -ForegroundColor Cyan
    Write-Host "dotnet pack src\OneLine.Cli\OneLine.Cli.csproj" -ForegroundColor Gray
} else {
    Write-Host "`n=== BUILD ECHOUE - voir erreurs ci-dessus ===" -ForegroundColor Red
}

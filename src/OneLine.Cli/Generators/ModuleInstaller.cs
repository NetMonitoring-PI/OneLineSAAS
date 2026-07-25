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

<div align="center">

# One Line SaaS Kit

**Un backend SaaS complet en quelques minutes**

[![.NET](https://img.shields.io/badge/.NET-9.0-blue)](https://dotnet.microsoft.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![NuGet](https://img.shields.io/badge/NuGet-coming_soon-orange)](https://nuget.org)

</div>

---

## Le probleme

Chaque nouveau projet SaaS backend oblige les developpeurs a reconfigurer
les memes composants depuis zero : auth, multi-tenancy, billing, securite,
observabilite. Ces etapes repetitives prennent 3 a 7 jours avant d ecrire
la premiere ligne de logique metier.

## La solution

```bash
saas new MonStartup
saas add auth
saas add tenant
saas add billing
saas add security
saas add logging
saas add ai
```

Un backend SaaS complet en Clean Architecture, pret pour la production,
en moins de 15 minutes.

---

## Architecture

```
OneLine.SaasKit/
â”œâ”€â”€ src/
â”‚   â”œâ”€â”€ Shared/
â”‚   â”‚   â””â”€â”€ OneLine.Shared.Domain/     <- Result<T>, BaseEntity, interfaces
â”‚   â”œâ”€â”€ Modules/
â”‚   â”‚   â”œâ”€â”€ Auth/                      <- JWT, RBAC, refresh tokens
â”‚   â”‚   â”œâ”€â”€ Tenants/                   <- Multi-tenancy, isolation DB
â”‚   â”‚   â”œâ”€â”€ Billing/                   <- Stripe, abonnements, webhooks
â”‚   â”‚   â”œâ”€â”€ Security/                  <- Rate limiting, brute force, API Keys
â”‚   â”‚   â”œâ”€â”€ Observability/             <- Serilog, Prometheus, CorrelationId
â”‚   â”‚   â””â”€â”€ AI/                        <- LLM, chat, usage tracking
â”‚   â”œâ”€â”€ OneLine.API/                   <- ASP.NET Core Web API
â”‚   â””â”€â”€ OneLine.Cli/                   <- CLI Tool
â””â”€â”€ tools/
    â””â”€â”€ OneLine.Migrations/            <- EF Core migrations
```

Chaque module suit **Clean Architecture** :
- `Domain/` : entites, interfaces (0 dependance)
- `Application/` : use cases, DTOs, CQRS avec MediatR
- `Infrastructure/` : EF Core, services externes

---

## Demarrage rapide

### Prerequis

- .NET 9 SDK
- Docker Desktop
- Git

### Installation

```bash
# 1. Cloner le repo
git clone https://github.com/votre-username/OneLine.SaasKit.git
cd OneLine.SaasKit

# 2. Lancer la base de donnees
docker compose up -d

# 3. Appliquer les migrations
dotnet ef database update --project src/Modules/Auth/OneLine.Auth.Infrastructure ...

# 4. Lancer l API
dotnet run --project src/OneLine.API/OneLine.API.csproj
```

### CLI

```bash
# Installer le CLI
dotnet tool install -g OneLine.Cli

# Creer un nouveau projet SaaS
saas new MonApplication

# Ajouter des modules
saas add auth
saas add tenant
saas add billing
```

---

## Modules

| Module | Description | Commande CLI |
|--------|-------------|--------------|
| Auth | JWT + refresh tokens + RBAC | `saas add auth` |
| Tenants | Multi-tenancy + isolation DB | `saas add tenant` |
| Billing | Stripe + abonnements + webhooks | `saas add billing` |
| Security | Rate limiting + brute force + API Keys | `saas add security` |
| Observability | Serilog + Prometheus + CorrelationId | `saas add logging` |
| AI | LLM + chat + usage tracking + quotas | `saas add ai` |

---

## API Endpoints

| Methode | Endpoint | Description |
|---------|----------|-------------|
| POST | `/api/auth/register` | Creer un compte |
| POST | `/api/auth/login` | Se connecter |
| POST | `/api/tenants` | Creer un tenant |
| GET | `/api/tenants/{id}` | Obtenir un tenant |
| POST | `/api/billing/subscribe` | S abonner a un plan |
| GET | `/api/billing/{tenantId}` | Voir l abonnement |
| POST | `/api/billing/webhook` | Webhook Stripe |
| POST | `/api/ai/chat` | Envoyer un message a l IA |
| GET | `/api/ai/usage/{tenantId}` | Stats d utilisation IA |
| GET | `/metrics` | Metriques Prometheus |

---

## Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=oneline_saaskit;..."
  },
  "Jwt": {
    "SecretKey": "votre-cle-secrete-minimum-32-chars",
    "Issuer": "OneLine.API",
    "Audience": "OneLine.Client",
    "AccessTokenExpiryMinutes": 15
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "AI": {
    "ApiKey": "sk-...",
    "Model": "gpt-4o-mini"
  }
}
```

---

## Stack technique

| Composant | Technologie |
|-----------|-------------|
| Framework | ASP.NET Core 9 |
| ORM | EF Core 9 + PostgreSQL |
| Auth | ASP.NET Identity + JWT |
| CQRS | MediatR |
| Validation | FluentValidation |
| Logging | Serilog |
| Metriques | Prometheus |
| Paiement | Stripe.net |
| IA | Azure.AI.OpenAI |
| Tests | xUnit + Testcontainers |
| CLI | System.CommandLine + Spectre.Console |

---

## Design Patterns

- **Strategy** : ITenantResolver (3 implementations), ILLMService (4 providers)
- **Repository + Unit of Work** : acces donnees abstrait
- **Mediator (CQRS)** : Commands et Queries separes via MediatR
- **Factory Method** : creation des entites (AppUser.Create, Tenant.Create)
- **Chain of Responsibility** : pipeline middleware
- **Options Pattern** : configuration fortement typee
- **Result Pattern** : gestion des erreurs sans exceptions metier

---

## Projets similaires

| Projet | Technologie | Notre avantage |
|--------|-------------|----------------|
| ABP Framework | .NET | Plus leger, CLI-first, IA native |
| Laravel Spark | PHP | Ecosysteme .NET |
| SaasRock | Node.js | .NET + IA integree |
| cookiecutter-django | Python | CLI modulaire intelligent |

---

## Licence

MIT - Voir [LICENSE](LICENSE)

---

<div align="center">

Fait avec par Imane | EHEI Oujda | PFA 2025-2026

</div>

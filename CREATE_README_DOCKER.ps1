# ============================================================
# Script README + Docker Compose Final
# Executer depuis : C:\Users\DELL\Projects\OneLine.SaasKit
# ============================================================

Write-Host "=== README + Docker Compose ===" -ForegroundColor Cyan

# ── README.md ────────────────────────────────────────────────
Write-Host "`n[1/2] Creation README.md..." -ForegroundColor Yellow

Set-Content -Path "README.md" -Encoding UTF8 -Value @'
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
├── src/
│   ├── Shared/
│   │   └── OneLine.Shared.Domain/     <- Result<T>, BaseEntity, interfaces
│   ├── Modules/
│   │   ├── Auth/                      <- JWT, RBAC, refresh tokens
│   │   ├── Tenants/                   <- Multi-tenancy, isolation DB
│   │   ├── Billing/                   <- Stripe, abonnements, webhooks
│   │   ├── Security/                  <- Rate limiting, brute force, API Keys
│   │   ├── Observability/             <- Serilog, Prometheus, CorrelationId
│   │   └── AI/                        <- LLM, chat, usage tracking
│   ├── OneLine.API/                   <- ASP.NET Core Web API
│   └── OneLine.Cli/                   <- CLI Tool
└── tools/
    └── OneLine.Migrations/            <- EF Core migrations
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
'@

Write-Host "README.md cree" -ForegroundColor Green

# ── Docker Compose ────────────────────────────────────────────
Write-Host "`n[2/2] Creation docker-compose.yml..." -ForegroundColor Yellow

Set-Content -Path "docker-compose.yml" -Encoding UTF8 -Value @'
services:

  # ── Base de donnees PostgreSQL ────────────────────────────
  postgres:
    image: postgres:16-alpine
    container_name: oneline_postgres
    environment:
      POSTGRES_DB: oneline_saaskit
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5433:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - oneline_network

  # ── Prometheus (metriques) ────────────────────────────────
  prometheus:
    image: prom/prometheus:latest
    container_name: oneline_prometheus
    ports:
      - "9090:9090"
    volumes:
      - ./docker/prometheus.yml:/etc/prometheus/prometheus.yml
      - prometheus_data:/prometheus
    command:
      - --config.file=/etc/prometheus/prometheus.yml
      - --storage.tsdb.retention.time=7d
    networks:
      - oneline_network
    depends_on:
      - postgres

  # ── Grafana (dashboards) ──────────────────────────────────
  grafana:
    image: grafana/grafana:latest
    container_name: oneline_grafana
    ports:
      - "3000:3000"
    environment:
      GF_SECURITY_ADMIN_PASSWORD: admin
      GF_USERS_ALLOW_SIGN_UP: "false"
    volumes:
      - grafana_data:/var/lib/grafana
    networks:
      - oneline_network
    depends_on:
      - prometheus

volumes:
  postgres_data:
  prometheus_data:
  grafana_data:

networks:
  oneline_network:
    driver: bridge
'@

# ── Config Prometheus ─────────────────────────────────────────
New-Item -ItemType Directory -Path "docker" -Force | Out-Null

Set-Content -Path "docker\prometheus.yml" -Encoding UTF8 -Value @'
global:
  scrape_interval: 15s
  evaluation_interval: 15s

scrape_configs:
  - job_name: oneline_api
    static_configs:
      - targets:
          - host.docker.internal:5160
    metrics_path: /metrics
'@

# ── .dockerignore ─────────────────────────────────────────────
Set-Content -Path ".dockerignore" -Encoding UTF8 -Value @'
**/.git
**/.vs
**/bin
**/obj
**/*.user
**/logs
**/*.md
'@

Write-Host "Docker Compose cree" -ForegroundColor Green

# ── Lancer les services ───────────────────────────────────────
Write-Host "`n[3/3] Lancement des services Docker..." -ForegroundColor Yellow
docker compose up -d

Write-Host "`n=== TERMINE ===" -ForegroundColor Green
Write-Host "`nServices disponibles :" -ForegroundColor Cyan
Write-Host "  PostgreSQL  : localhost:5433" -ForegroundColor White
Write-Host "  Prometheus  : http://localhost:9090" -ForegroundColor White
Write-Host "  Grafana     : http://localhost:3000 (admin/admin)" -ForegroundColor White
Write-Host "  API         : http://localhost:5160/swagger" -ForegroundColor White
Write-Host "  Metriques   : http://localhost:5160/metrics" -ForegroundColor White
Write-Host "`nCommit :" -ForegroundColor Cyan
Write-Host "  git add ." -ForegroundColor Gray
Write-Host "  git commit -m 'docs: add README and Docker Compose with Prometheus + Grafana'" -ForegroundColor Gray
Write-Host "  git push origin main" -ForegroundColor Gray

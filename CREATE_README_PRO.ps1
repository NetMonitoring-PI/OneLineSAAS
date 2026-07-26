# ============================================================
# README Professionnel - One Line SaaS Kit
# ============================================================

Write-Host "=== Creation README Professionnel ===" -ForegroundColor Cyan

Set-Content -Path "README.md" -Encoding UTF8 -Value @'
<div align="center">

<img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet" />
<img src="https://img.shields.io/badge/License-MIT-22C55E?style=for-the-badge" />
<img src="https://img.shields.io/badge/NuGet-coming_soon-F97316?style=for-the-badge&logo=nuget" />
<img src="https://img.shields.io/badge/PostgreSQL-16-4169E1?style=for-the-badge&logo=postgresql" />

# One Line SaaS Kit

### Un backend SaaS complet en quelques minutes — pas en plusieurs jours.

[Demarrage rapide](#demarrage-rapide) •
[Modules](#modules) •
[CLI](#cli) •
[API](#api-endpoints) •
[Architecture](#architecture)

</div>

---

## Pourquoi One Line SaaS Kit ?

Chaque nouveau projet SaaS backend oblige les developpeurs a reconfigurer
les memes briques techniques depuis zero.

| Sans le kit | Avec le kit |
|-------------|-------------|
| Auth JWT from scratch : 2 jours | `saas add auth` : 30 secondes |
| Multi-tenancy : 1 semaine | `saas add tenant` : 30 secondes |
| Stripe + webhooks : 1 semaine | `saas add billing` : 30 secondes |
| Rate limiting + securite : 2 jours | `saas add security` : 30 secondes |
| Logging + metriques : 2 jours | `saas add logging` : 30 secondes |
| Integration IA : 1 semaine | `saas add ai` : 30 secondes |
| **Total : 3 a 7 jours** | **Total : moins de 15 minutes** |

> Equivalent .NET de **Laravel Spark** et **SaasRock** — avec une couche IA native.

---

## Demarrage rapide

### Prerequis

```bash
dotnet --version   # >= 9.0
docker --version   # Docker Desktop installe
git --version
```

### Option 1 — CLI (recommande)

```bash
# Installer le CLI
dotnet tool install -g OneLine.Cli

# Creer un nouveau projet SaaS complet
saas new MonStartup

# Naviguer dans le projet
cd MonStartup

# Ajouter les modules voulus
saas add auth
saas add tenant
saas add billing
saas add security
saas add logging
saas add ai

# Lancer
docker compose up -d
dotnet run --project src/MonStartup.API
```

### Option 2 — Cloner ce repo

```bash
git clone https://github.com/votre-username/OneLine.SaasKit.git
cd OneLine.SaasKit

# Lancer la base de donnees
docker compose up -d

# Appliquer les migrations (Auth)
dotnet ef database update \
  --project src/Modules/Auth/OneLine.Auth.Infrastructure \
  --startup-project tools/OneLine.Migrations \
  --context AuthDbContext

# Appliquer les migrations (Tenants)
dotnet ef database update \
  --project src/Modules/Tenants/OneLine.Tenants.Infrastructure \
  --startup-project tools/OneLine.Migrations \
  --context TenantsDbContext

# Appliquer les migrations (Billing)
dotnet ef database update \
  --project src/Modules/Billing/OneLine.Billing.Infrastructure \
  --startup-project tools/OneLine.Migrations \
  --context BillingDbContext

# Appliquer les migrations (AI)
dotnet ef database update \
  --project src/Modules/AI/OneLine.AI.Infrastructure \
  --startup-project tools/OneLine.Migrations \
  --context AIDbContext

# Lancer l API
dotnet run --project src/OneLine.API/OneLine.API.csproj

# Ouvrir Swagger
# http://localhost:5160/swagger
```

---

## Modules

### Auth — Authentification complete

```bash
saas add auth
```

**Ce que ca fait :**
- JWT access token (15 min) + refresh token rotation securisee (7 jours)
- RBAC : roles et permissions par claims JWT
- Protection lockout apres 5 tentatives echouees
- Endpoints : register, login, refresh, logout, revoke

**Endpoints :**
```
POST /api/auth/register   Creer un compte
POST /api/auth/login      Se connecter -> JWT + RefreshToken
POST /api/auth/refresh    Renouveler le JWT
POST /api/auth/logout     Deconnecter
```

**Configuration :**
```json
"Jwt": {
  "SecretKey": "votre-cle-32-chars-minimum",
  "Issuer": "VotreAPI",
  "Audience": "VotreClient",
  "AccessTokenExpiryMinutes": 15,
  "RefreshTokenExpiryDays": 7
}
```

---

### Tenants — Multi-tenancy

```bash
saas add tenant
```

**Ce que ca fait :**
- Isolation complete des donnees entre clients (Global Query Filter EF Core)
- 3 strategies de resolution du tenant :
  - Header HTTP : `X-Tenant-Id: <guid>`
  - Claim JWT : `tenant_id` dans le token
  - Sous-domaine : `client1.votreapp.com`
- Provisioning automatique a l inscription
- Trial de 14 jours par defaut

**Endpoints :**
```
POST /api/tenants          Creer un tenant
GET  /api/tenants/{id}     Obtenir un tenant
```

**Utilisation :**
```bash
# Resoudre le tenant via header
curl -H "X-Tenant-Id: votre-tenant-id" http://localhost:5160/api/...

# Resoudre via JWT (automatique si tenant_id dans le token)
curl -H "Authorization: Bearer votre-jwt" http://localhost:5160/api/...
```

---

### Billing — Paiement Stripe

```bash
saas add billing
```

**Ce que ca fait :**
- Integration Stripe complete (Customer, Subscription, Invoice)
- Gestion des plans tarifaires (Free, Starter, Pro, Enterprise)
- Webhooks Stripe automatiques :
  - `invoice.payment_succeeded` -> activer abonnement
  - `invoice.payment_failed` -> marquer PastDue
  - `customer.subscription.deleted` -> annuler
- Middleware HTTP 402 si abonnement expire

**Endpoints :**
```
POST   /api/billing/subscribe    S abonner a un plan
GET    /api/billing/{tenantId}   Voir l abonnement actif
DELETE /api/billing/{tenantId}   Annuler l abonnement
POST   /api/billing/webhook      Recevoir les events Stripe
```

**Configuration :**
```json
"Stripe": {
  "SecretKey": "sk_test_...",
  "PublishableKey": "pk_test_...",
  "WebhookSecret": "whsec_..."
}
```

---

### Security — Protection API

```bash
saas add security
```

**Ce que ca fait :**
- Rate limiting : 60 requetes/minute par IP -> HTTP 429
- Protection brute force : lockout apres 5 tentatives -> 15 min
- API Keys : generation, validation, revocation (header X-Api-Key)
- CORS configurable

**Configuration :**
```json
"Security": {
  "MaxRequestsPerMinutePerIp": 60,
  "MaxFailedLoginAttempts": 5,
  "LockoutDurationMinutes": 15
}
```

---

### Observability — Monitoring

```bash
saas add logging
```

**Ce que ca fait :**
- Logging structure Serilog (Console + File rotation quotidienne)
- X-Correlation-Id unique sur chaque requete HTTP
- Metriques Prometheus sur `/metrics`
- Log de chaque requete (methode + path + status + duree ms)
- Alerte automatique si requete > 1000ms

**Acces :**
```
GET /metrics              Metriques Prometheus
http://localhost:9090     Interface Prometheus
http://localhost:3001     Grafana (admin/admin)
```

---

### AI — Intelligence Artificielle

```bash
saas add ai
```

**Ce que ca fait :**
- Abstraction multi-provider LLM :
  - OpenAI (GPT-4o, GPT-4o-mini)
  - Mode Mock pour les tests (sans cle API)
- Conversations multi-tours avec historique par tenant
- Tracking de tokens par tenant (quota mensuel)
- Quota middleware : HTTP 429 si quota depasse
- Multi-tenancy : chaque tenant a ses propres conversations

**Endpoints :**
```
POST /api/ai/chat              Envoyer un message a l IA
GET  /api/ai/usage/{tenantId}  Stats tokens/quota/cout
```

**Exemple :**
```json
POST /api/ai/chat
{
  "tenantId": "3fa85f64-...",
  "message": "Explique-moi Clean Architecture",
  "systemPrompt": "Tu es un expert .NET"
}

Response:
{
  "conversationId": "...",
  "content": "Clean Architecture est...",
  "tokensUsed": 245,
  "monthlyTokensUsed": 245,
  "monthlyQuota": 50000,
  "model": "gpt-4o-mini",
  "provider": "OpenAI"
}
```

**Configuration :**
```json
"AI": {
  "ApiKey": "sk-...",
  "Model": "gpt-4o-mini",
  "MaxTokens": 2000,
  "Temperature": "0.7"
}
```

> Si `ApiKey` est vide -> mode Mock automatique (parfait pour le developpement)

---

## Architecture

```
src/
├── Shared/
│   └── OneLine.Shared.Domain/        <- Result<T>, BaseEntity, interfaces
│
├── Modules/
│   ├── Auth/
│   │   ├── OneLine.Auth.Domain/      <- AppUser, RefreshToken, erreurs
│   │   ├── OneLine.Auth.Application/ <- Login, Register, CQRS
│   │   └── OneLine.Auth.Infrastructure/ <- EF Core, JWT, Repositories
│   │
│   ├── Tenants/                      <- Meme structure
│   ├── Billing/                      <- Meme structure
│   ├── Security/                     <- Infrastructure uniquement
│   ├── Observability/                <- Infrastructure uniquement
│   └── AI/                           <- Meme structure
│
├── OneLine.API/                      <- Controllers, middleware, Program.cs
└── OneLine.Cli/                      <- CLI Tool (saas new, saas add)
```

Chaque module suit **Clean Architecture** :

```
Domain      <- entites, interfaces (0 dependance externe)
    |
Application <- use cases, DTOs, CQRS via MediatR
    |
Infrastructure <- EF Core, services, repositories
    |
API         <- controllers, middleware
```

**Patterns implementes :**
- Strategy : ITenantResolver, ILLMService
- Repository + Unit of Work
- Mediator (CQRS) via MediatR
- Factory Method : AppUser.Create(), Tenant.Create()
- Chain of Responsibility : pipeline middleware
- Options Pattern : configuration typee
- Result Pattern : pas d exceptions metier

---

## Middleware Pipeline

```
Request
  -> CorrelationId    (X-Correlation-Id unique)
  -> RequestLogging   (log methode + status + duree)
  -> RateLimit        (429 si > 60 req/min)
  -> ApiKey           (auth via X-Api-Key)
  -> TenantResolver   (detecte le tenant)
  -> Authentication   (valide le JWT)
  -> Authorization    (verifie les roles)
  -> AI Quota         (429 si quota tokens depasse)
  -> Controller
```

---

## Stack technique

| Composant | Technologie | Version |
|-----------|-------------|---------|
| Framework | ASP.NET Core | 9.0 LTS |
| ORM | EF Core + PostgreSQL | 9.0 |
| Auth | ASP.NET Identity + JWT | 9.0 |
| CQRS | MediatR | 12.2 |
| Validation | FluentValidation | 11.9 |
| Logging | Serilog | 8.0 |
| Metriques | prometheus-net | 8.2 |
| Paiement | Stripe.net | 46.x |
| IA | Azure.AI.OpenAI | 2.1 |
| CLI | System.CommandLine + Spectre.Console | latest |
| Tests | xUnit + Testcontainers | latest |

---

## Comparaison

| | One Line SaaS Kit | ABP Framework | Laravel Spark | SaasRock |
|--|--|--|--|--|
| Langage | .NET 9 | .NET | PHP | Node.js |
| CLI | Oui | Non | Non | Non |
| Multi-tenancy | Oui | Oui | Oui | Oui |
| Billing Stripe | Oui | Payant | Oui | Oui |
| Module IA natif | Oui | Non | Non | Non |
| Open source | Oui | Partiel | Non | Non |
| Complexite | Faible | Elevee | Moyenne | Moyenne |

---

## Licence

MIT - libre pour usage personnel et commercial.

---

<div align="center">

**One Line SaaS Kit** — Developpe par Imane | EHEI Oujda | PFA 2025-2026

*Inspire de Laravel Spark et SaasRock — concu pour l ecosysteme .NET*

</div>
'@

Write-Host "README.md professionnel cree" -ForegroundColor Green
Write-Host "`nN oublie pas de remplacer votre-username dans les liens GitHub !" -ForegroundColor Yellow

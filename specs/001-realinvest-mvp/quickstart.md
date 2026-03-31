# Quickstart: MshNawy (مش ناوي) MVP

**Branch**: `001-realinvest-mvp` | **Date**: 2026-02-28 | **Updated**: 2026-03-13

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | LTS (8.0+) | Backend runtime |
| Node.js | LTS (20+) | Frontend tooling |
| Angular CLI | 18.x | Frontend framework |
| SQL Server | Latest stable | Database (LocalDB or full instance) |
| Git | 2.30+ | Version control |

> **Note**: ABP CLI and DbMigrator are not used in this project. Migrations are applied via `dotnet ef` against the `MshNawy.EntityFrameworkCore` project with `MshNawy.HttpApi.Host` as the startup project.

## Project Structure (Actual)

### Backend (`aspnet-core/src/`)

```text
MshNawy.Domain.Shared/        — Enums, constants, error codes, localization
MshNawy.Domain/                — Entities, domain services, interfaces
├── Identity/                  — AppUser, IAppUserRepository, OtpService
├── Wallet/                    — LedgerEntry, LedgerService, BalanceCalculator
├── Fees/                      — FeePolicy, FeeCalculator
└── Shared/                    — IFileStorageService, FileStorageOptions
MshNawy.Application.Contracts/ — DTOs, service interfaces
├── Identity/                  — KYC DTOs
MshNawy.Application/           — Application services, AutoMapper, validators
├── Identity/                  — KYC application service
MshNawy.EntityFrameworkCore/   — DbContext, migrations, repositories
├── Infrastructure/            — IFileStorageService implementations
├── Repositories/              — Custom repositories
├── Migrations/                — EF Core migrations
MshNawy.HttpApi/               — API controllers
├── Identity/                  — KYC/auth controllers
MshNawy.HttpApi.Host/          — Host startup, configuration
```

### Backend Tests (`aspnet-core/test/`)

```text
MshNawy.Domain.Tests/
├── Fees/                      — FeeCalculatorTests
├── Identity/                  — Identity-related tests
└── Wallet/                    — BalanceCalculatorTests, LedgerServiceTests
MshNawy.Application.Tests/     — Integration tests
```

### Frontend (`angular/src/app/`)

```text
shared/
├── components/                — Reusable UI components
├── guards/                    — Auth/KYC guards
├── models/                    — TypeScript interfaces (identity.models.ts)
└── pipes/                     — Formatting pipes
onboarding/
├── login/                     — OTP login flow
├── kyc/                       — KYC submission form
└── kyc-status/                — KYC status display
admin/
└── kyc-review/                — Admin KYC review screen
mock/
├── browser.ts                 — MSW setup
├── handlers/                  — MSW request handlers (base.ts, identity.handlers.ts)
└── data/                      — Seed data (seed.ts)
```

### Tooling

```text
angular/.storybook/            — Storybook configuration (main.ts, preview.ts)
```

## Key Configuration

### appsettings.json (Backend — `MshNawy.HttpApi.Host`)

```json
{
  "App": {
    "SelfUrl": "http://localhost:5000"
  },
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=MshNawy;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "FileStorage": {
    "BasePath": "./uploads",
    "MaxFileSizeBytes": 5242880,
    "AllowedExtensions": [".jpg", ".jpeg", ".png"]
  },
  "Jwt": {
    "Issuer": "MshNawy",
    "Audience": "MshNawy",
    "SigningKey": "dev-signing-key-change-me",
    "ExpiresInMinutes": 60
  }
}
```

**Ports** (from `launchSettings.json`):
- HTTPS: `https://localhost:58155`
- HTTP: `http://localhost:58156`

### environment.ts (Frontend)

```typescript
export const environment = {
  production: false,
  mockApiEnabled: true,   // Toggle MSW on/off
  apiUrl: 'http://localhost:3000/api'
};
```

> **Note**: When `mockApiEnabled` is `true`, MSW intercepts all API calls. Set to `false` and update `apiUrl` to point to the backend (e.g., `https://localhost:58155`) for real API mode.

## Run Commands

### First-Time Setup

```bash
# 1. Restore backend dependencies
cd aspnet-core
dotnet restore

# 2. Create/update the database via EF Core migrations
cd aspnet-core
dotnet ef database update \
  --project src/MshNawy.EntityFrameworkCore \
  --startup-project src/MshNawy.HttpApi.Host

# 3. Install frontend dependencies
cd angular
npm install

# 4. (Optional) Initialize MSW service worker
cd angular
npx msw init public/ --save
```

### Daily Development

```bash
# -- Mock mode (frontend only, no backend required) -------------------
cd angular
ng serve
# Opens: http://localhost:4200
# MSW intercepts all API calls with deterministic mock data

# -- Full stack mode ---------------------------------------------------
# Terminal 1 — Backend
cd aspnet-core/src/MshNawy.HttpApi.Host
dotnet run
# HTTPS: https://localhost:58155
# HTTP:  http://localhost:58156
# Swagger: https://localhost:58155/swagger

# Terminal 2 — Frontend (real API)
# First: edit angular/src/environments/environment.ts
#   → set mockApiEnabled: false
#   → set apiUrl: 'https://localhost:58155'
cd angular
ng serve

# -- Admin panel -------------------------------------------------------
# Navigate to http://localhost:4200/admin after login
# Currently implements: KYC review
```

### Database

```bash
# Create a new EF Core migration
cd aspnet-core
dotnet ef migrations add <MigrationName> \
  --project src/MshNawy.EntityFrameworkCore \
  --startup-project src/MshNawy.HttpApi.Host

# Apply migrations
cd aspnet-core
dotnet ef database update \
  --project src/MshNawy.EntityFrameworkCore \
  --startup-project src/MshNawy.HttpApi.Host

# Reset database (dev only)
cd aspnet-core
dotnet ef database drop --force \
  --project src/MshNawy.EntityFrameworkCore \
  --startup-project src/MshNawy.HttpApi.Host
dotnet ef database update \
  --project src/MshNawy.EntityFrameworkCore \
  --startup-project src/MshNawy.HttpApi.Host
```

### Testing

```bash
# -- Backend -----------------------------------------------------------
cd aspnet-core
dotnet test                                        # all tests
dotnet test --filter "FullyQualifiedName~Domain"   # domain tests only

# -- Component catalog -------------------------------------------------
cd angular
npm run storybook                                  # http://localhost:6006
```

> **Not yet implemented**: Frontend unit tests (no test runner configured), E2E tests (no Playwright setup).

### Build & Quality Checks

```bash
# Backend build
cd aspnet-core
dotnet build

# Frontend build
cd angular
ng build --configuration=production
```

## Development Workflow

### Frontend-First (Constitution Principle VI)

1. **Define contracts** — Write DTOs in `Application.Contracts` first
2. **Create MSW handlers** — Mock all API endpoints in `angular/src/app/mock/handlers/`
3. **Build Angular UI** — Develop against mocks, verify with Storybook
4. **Write backend** — Implement application services matching the contracts
5. **Integrate** — Set `mockApiEnabled: false` in `environment.ts`, point to real API

### Implementation Status

What exists today:

| Layer | Implemented | Planned |
|-------|------------|---------|
| **Domain** | Identity (AppUser, OTP), Wallet (Ledger, Balance), Fees (Policy, Calculator) | Deposits, Withdrawals, Offerings, Orders, Portfolio, PropertySales, Support, Notifications |
| **Application** | Identity (KYC service) | All other services |
| **HttpApi** | Identity controllers | All other controllers |
| **EF Core** | DbContext, migrations (Initial + Fees + KYC), repositories, file storage | Remaining entity configurations |
| **Angular** | Onboarding (login, KYC, KYC status), Admin (KYC review), shared components/guards/pipes, MSW setup | Wallet, Offerings, Subscription, Portfolio, PropertySales, Support, Notifications |
| **Tests** | FeeCalculator, BalanceCalculator, LedgerService (domain tests) | Application integration tests, API tests, frontend tests, E2E |

### Known Issues

- `angular/package.json` name is still `"realinvest-angular"` (not yet renamed to `"mshn-nawy"`)
- `angular/angular.json` project name is still `"realinvest-angular"`
- No build budget configured in `angular.json` (target: 250KB gzip)
- No frontend test runner configured (Jest or Karma)
- No E2E test infrastructure (Playwright)

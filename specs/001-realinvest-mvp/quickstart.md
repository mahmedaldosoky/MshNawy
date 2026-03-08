# Quickstart: MshNawy (مش ناوي) MVP

**Branch**: `001-realinvest-mvp` | **Date**: 2026-02-28 | **Updated**: 2026-03-08

## Prerequisites

| Tool | Version | Purpose |
|------|---------|---------|
| .NET SDK | Latest LTS (8.0+) | Backend runtime |
| Node.js | Latest LTS (20+) | Frontend tooling |
| Angular CLI | Latest LTS | Frontend framework |
| ABP CLI | Latest | Project scaffolding, module management |
| SQL Server or PostgreSQL | Latest stable | Database |
| Git | 2.30+ | Version control |

## Project Scaffolding

### 1. Create ABP Solution

```bash
abp new MshNawy -t app -u angular --database-provider ef -dbms SqlServer --mobile none
```

This generates the standard ABP layered structure:
- `aspnet-core/src/MshNawy.Domain.Shared/` — Enums, constants, shared DTOs
- `aspnet-core/src/MshNawy.Domain/` — Entities, aggregates, domain services, state machines
- `aspnet-core/src/MshNawy.Application.Contracts/` — DTOs, application service interfaces
- `aspnet-core/src/MshNawy.Application/` — Application services, AutoMapper profiles
- `aspnet-core/src/MshNawy.EntityFrameworkCore/` — EF Core DbContext, migrations, repositories
- `aspnet-core/src/MshNawy.HttpApi/` — API controllers
- `aspnet-core/src/MshNawy.HttpApi.Host/` — Host startup, configuration
- `aspnet-core/src/MshNawy.DbMigrator/` — Database migration runner
- `angular/` — Angular frontend application

### 2. Verify Scaffolding

```bash
cd aspnet-core
dotnet build
cd ../angular
npm install
ng serve
```

### 3. Install Additional Dependencies

**Backend** (NuGet):
```bash
# In aspnet-core/src/MshNawy.Application/
dotnet add package FluentValidation.DependencyInjectionExtensions
```

**Frontend** (npm):
```bash
cd angular
npm install msw --save-dev          # Mock Service Worker for frontend-first development
npm install @storybook/angular --save-dev  # Component catalog
```

## Development Workflow

### Frontend-First (Constitution Principle VI)

1. **Define contracts** — Write DTOs in `Application.Contracts` first
2. **Create MSW handlers** — Mock all API endpoints with deterministic data
3. **Build Angular UI** — Develop against mocks, verify with Storybook
4. **Write backend** — Implement application services matching the exact same contracts
5. **Integrate** — Switch MSW off, point to real API

### Running in Mock Mode

```bash
cd angular
# MSW is configured to intercept in development mode
ng serve  # Starts with mock API handlers active
```

### Running Full Stack

```bash
# Terminal 1: Backend
cd aspnet-core/src/MshNawy.HttpApi.Host
dotnet run

# Terminal 2: Frontend
cd angular
ng serve --configuration=production  # Disables MSW, uses real API
```

## Key Configuration

### appsettings.json (Backend)

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=MshNawy;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "FileStorage": {
    "BasePath": "./uploads",
    "MaxFileSizeBytes": 5242880,
    "AllowedExtensions": [".jpg", ".jpeg", ".png"]
  },
  "Otp": {
    "ExpirationSeconds": 180,
    "MaxAttemptsPerWindow": 5,
    "WindowMinutes": 15,
    "LockoutMinutes": 30,
    "CodeLength": 6
  }
}
```

### environment.ts (Frontend)

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:44300',
  mockEnabled: true,  // Toggle MSW on/off
  locale: 'ar-EG',
  currency: 'EGP'
};
```

## Domain Structure (Folder Organization)

```
aspnet-core/src/MshNawy.Domain/
├── Identity/
│   ├── KycStatus.cs (enum - in Domain.Shared)
│   └── OtpService.cs
├── Wallet/
│   ├── LedgerEntry.cs
│   ├── LedgerEntryType.cs (enum - in Domain.Shared)
│   ├── LedgerService.cs
│   └── BalanceCalculator.cs
├── Deposits/
│   ├── Deposit.cs
│   ├── DepositStatus.cs (enum - in Domain.Shared)
│   └── DepositManager.cs
├── Withdrawals/
│   ├── Withdrawal.cs
│   ├── WithdrawalStatus.cs (enum - in Domain.Shared)
│   └── WithdrawalManager.cs
├── Offerings/
│   ├── Offering.cs
│   ├── OfferingFinancialModel.cs
│   ├── OfferingImage.cs
│   ├── OfferingStatus.cs (enum - in Domain.Shared)
│   └── ProjectionEngine.cs
├── Orders/
│   ├── InvestmentOrder.cs
│   ├── OrderStatus.cs (enum - in Domain.Shared)
│   ├── Installment.cs
│   ├── InstallmentStatus.cs (enum - in Domain.Shared)
│   ├── OrderManager.cs
│   └── InstallmentProcessor.cs
├── Portfolio/
│   ├── Holding.cs
│   └── StatementGenerator.cs
├── Exits/
│   ├── ExitRequest.cs
│   ├── ExitStatus.cs (enum - in Domain.Shared)
│   └── ExitManager.cs
├── Fees/
│   ├── FeePolicy.cs
│   └── FeeCalculator.cs
├── Support/
│   ├── SupportTicket.cs
│   ├── TicketMessage.cs
│   ├── TicketAttachment.cs
│   ├── TicketCategory.cs (enum - in Domain.Shared)
│   └── TicketStatus.cs (enum - in Domain.Shared)
├── Notifications/
│   ├── Notification.cs
│   ├── NotificationEventType.cs (enum - in Domain.Shared)
│   └── NotificationService.cs
└── Shared/
    └── RiskLevel.cs (enum - in Domain.Shared)
```

## Angular Structure

```
angular/src/app/
├── shared/
│   ├── components/     # Reusable UI components (all with Storybook stories)
│   ├── services/       # Shared services (auth, locale, notification)
│   ├── guards/         # KYC guard, auth guard
│   ├── pipes/          # EGP formatting, Arabic date, etc.
│   └── models/         # TypeScript interfaces matching API DTOs
├── onboarding/         # OTP login, KYC flow
├── wallet/             # Balances, deposits, withdrawals, transactions
├── offerings/          # Browse, detail, projections
├── subscription/       # Knowledge check, order flow
├── portfolio/          # Holdings, activity, statements
├── exits/              # Exit request flow
├── support/            # Ticket creation, thread view
├── notifications/      # Notification center
├── admin/              # Admin panel (lazy-loaded)
│   ├── kyc-review/
│   ├── deposit-review/
│   ├── withdrawal-review/
│   ├── order-settlement/
│   ├── exit-processing/
│   ├── support-management/
│   ├── offering-management/
│   └── fee-policy/
└── mock/               # MSW handlers and seed data
    ├── handlers/
    ├── data/
    └── browser.ts
```

## Run Commands

### First-Time Setup

```bash
# 1. Run database migrations (creates MshNawy DB and seeds initial data)
cd aspnet-core/src/MshNawy.DbMigrator
dotnet run
# Creates: MshNawy database, admin user, initial FeePolicy (entry 1%, payment 3%, exit 5%)

# 2. (Optional) Start the backend API
cd aspnet-core/src/MshNawy.HttpApi.Host
dotnet run
# API server:  https://localhost:44300
# Swagger UI:  https://localhost:44300/swagger

# 3. Install frontend dependencies
cd angular
npm install

# 4. (Optional) Initialize MSW service worker
cd angular
npx msw init public/ --save
```

### Daily Development

```bash
# ── Mock mode (frontend only, no backend required) ──────────────
cd angular
ng serve
# Opens: http://localhost:4200
# MSW intercepts all API calls with deterministic Arabic mock data

# ── Full stack mode ─────────────────────────────────────────────
# Terminal 1 — Backend
cd aspnet-core/src/MshNawy.HttpApi.Host
dotnet run

# Terminal 2 — Frontend (real API)
# First: edit angular/src/environments/environment.ts → set mockEnabled: false
cd angular
ng serve

# ── Admin panel ─────────────────────────────────────────────────
# Navigate to http://localhost:4200/admin after login with admin credentials
# Admin credentials are seeded by MshNawy.DbMigrator
```

### Database

```bash
# Create a new EF Core migration
cd aspnet-core
dotnet ef migrations add <MigrationName> \
  --project src/MshNawy.EntityFrameworkCore \
  --startup-project src/MshNawy.HttpApi.Host

# Apply migrations directly (alternative to DbMigrator)
cd aspnet-core
dotnet ef database update \
  --project src/MshNawy.EntityFrameworkCore \
  --startup-project src/MshNawy.HttpApi.Host

# Reset database (dev only)
cd aspnet-core
dotnet ef database drop --force \
  --project src/MshNawy.EntityFrameworkCore \
  --startup-project src/MshNawy.HttpApi.Host
dotnet run --project src/MshNawy.DbMigrator
```

### Testing

```bash
# ── Backend ─────────────────────────────────────────────────────
cd aspnet-core
dotnet test                                           # all tests
dotnet test --filter "Category=Unit"                  # unit tests only
dotnet test --filter "Category=Integration"           # integration tests only
dotnet test --collect:"XPlat Code Coverage"           # with coverage report

# ── Frontend ────────────────────────────────────────────────────
cd angular
ng test                                               # unit tests (Jest, watch mode)
ng test --watch=false --code-coverage                 # single run with coverage

# ── Component catalog ───────────────────────────────────────────
cd angular
npm run storybook                                     # http://localhost:6006

# ── E2E (requires both backend + frontend running) ──────────────
cd angular
npx playwright test                                   # headless
npx playwright test --headed                          # with browser UI
npx playwright test --ui                              # Playwright interactive UI
```

### Build & Quality Checks

```bash
# Production build (enforces ≤250KB bundle budget)
cd angular
ng build --configuration=production

# Lint checks
cd angular
ng lint                                               # ESLint (includes RTL/localization rules)
cd aspnet-core
dotnet build -warnaserror                             # zero-warning strict mode

# Bundle size analysis
cd angular
ng build --configuration=production --stats-json
npx webpack-bundle-analyzer dist/*/stats.json
```

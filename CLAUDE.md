# MshNawy (مش ناوي) Development Guidelines

Last updated: 2026-03-08

## Active Technologies
- SQL Server (via ABP EF Core provider). All monetary values in piasters (bigint/long).
- .NET LTS (8.0+) + Angular LTS (17+/18+) + ABP Framework, Entity Framework Core, Angular Material, MSW (Mock Service Worker), Storybook, FluentValidation

## Project Structure

```text
aspnet-core/src/MshNawy.*/   — Backend projects (Domain, Application, EFCore, HttpApi, Host, DbMigrator)
aspnet-core/test/MshNawy.*/  — Backend test projects
angular/                     — Angular frontend (Arabic RTL, MSW mock layer, Storybook)
specs/001-realinvest-mvp/    — Feature spec, plan, data model, API contracts, tasks
```

## Commands

```bash
# Run database migrations
cd aspnet-core/src/MshNawy.DbMigrator && dotnet run

# Start API server  (https://localhost:44300)
cd aspnet-core/src/MshNawy.HttpApi.Host && dotnet run

# Start frontend — mock mode (no backend required)
cd angular && ng serve

# Start frontend — real API mode (set mockEnabled: false in environment.ts first)
cd angular && ng serve

# Backend tests
cd aspnet-core && dotnet test

# Frontend unit tests
cd angular && ng test

# Storybook
cd angular && npm run storybook

# E2E tests
cd angular && npx playwright test

# Production build (enforces ≤250KB bundle budget)
cd angular && ng build --configuration=production
```

## Code Style

- Arabic-only UI strings — use ABP localization pipes/services, never hard-coded strings
- All monetary values in piasters (long) in backend; convert to EGP only in presentation layer
- ABP layering: Domain → Application → HttpApi. No cross-layer dependency violations.
- All financial state changes via domain methods (guarded transitions). No direct property sets.
- PR size ≤ 400 LOC production code (excluding generated files, migrations, test files)

## Recent Changes
- 2026-03-08: Renamed project from RealInvest to MshNawy. Added .gitignore. Fixed plan.md with infrastructure layer, security architecture, presigned URL strategy, and complete run commands.
- 2026-02-28: Initial scaffold — ABP solution, Angular app, ledger, fee engine, shared components.

<!-- MANUAL ADDITIONS START -->
<!-- MANUAL ADDITIONS END -->

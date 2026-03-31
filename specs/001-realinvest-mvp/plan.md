# Implementation Plan: MshNawy (مش ناوي) — Egyptian Fractional Real Estate Investment Platform

**Branch**: `001-realinvest-mvp` | **Date**: 2026-02-28 | **Updated**: 2026-03-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-realinvest-mvp/spec.md`

## Summary

Build a full-stack web application for fractional real estate investing in the Egyptian market under the brand **MshNawy (مش ناوي)**. The platform enables Egyptian users to register via OTP, complete KYC, deposit funds, browse dynamically-projected real estate offerings, subscribe to investments with payment plans, manage a portfolio, and exit via property sale — all in Arabic-only RTL with EGP currency. Built on ABP Framework (.NET + Angular) with a double-entry ledger, configurable fee engine, and frontend-first development methodology using MSW for mock APIs.

## Technical Context

**Language/Version**: .NET LTS (8.0+) + Angular LTS (17+/18+)
**Primary Dependencies**: ABP Framework, Entity Framework Core, Angular Material, MSW (Mock Service Worker), Storybook, FluentValidation
**Storage**: SQL Server (via ABP EF Core provider). All monetary values in piasters (bigint/long).
**Testing**: xUnit + ABP test infrastructure (backend), Jest (frontend), Playwright (E2E)
**Target Platform**: Web (responsive: mobile 375px, tablet 768px, desktop 1280px)
**Project Type**: Web application (investor frontend + admin panel + backend API)
**Performance Goals**: API ≤ 200ms p95, LCP ≤ 2.5s, Angular bundle ≤ 250KB gzip, ≤ 10 DB round-trips/request
**Constraints**: 100 concurrent users MVP, Arabic-only RTL investor UI, EGP piasters only, no external payment integrations
**Scale/Scope**: ~15 entities, ~50 screens (investor + admin), 9 user stories, 36 functional requirements

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Evidence |
|-----------|--------|----------|
| I. Arabic-Only & Egyptian Market | **FAIL** — remediation in T184/T185 | FR-020: Arabic-only RTL, EGP formatting, ar-EG locale. FR-001: Egyptian phone OTP. FR-002: National ID KYC gate. **VIOLATION**: Hardcoded Arabic strings in Angular components (T184). "RealInvest" remnant in UI (T185). |
| II. Financial Integrity — Double-Entry Ledger | PASS | FR-005: All movements via ledger entries. Immutable posted entries. Compensating entries for corrections. LedgerEntry entity with debit/credit accounts, piasters, idempotency keys. |
| III. Atomic & Idempotent Financial Operations | **PARTIAL** — remediation in T187 | FR-013: All financial ops atomic + idempotent with UUID keys. State machines for all financial entities. Optimistic concurrency via ABP ConcurrencyStamp. **GAP**: Idempotency key middleware not yet implemented (T187 added to Phase 3b). |
| IV. Configurable Fee Policy | PASS | FR-014: DB-backed FeePolicy entity with entry/payment/exit fees. FR-023: Effective-date versioning. FR-015: Fee breakdowns before confirmation. No competing brand references in UI. |
| V. Dynamic Financial Projections | PASS | FR-008/009/010: Projection engine with explicit inputs, 3 scenario modes. FR-022: Disclaimer required. No static ROI. Pure domain service (ProjectionEngine). |
| VI. Frontend-First Delivery | PASS | FR-021: MSW mock layer with deterministic data. User Story 8 covers mock-first approach. Storybook for all shared components. Contract-first design. |
| VII. Code Quality & ABP Compliance | PASS | Single ABP module with DDD folder structure. Domain methods for state transitions. DTOs for all API boundaries. FluentValidation. ABP permission system for admin. PR ≤ 400 LOC. |

**Post-Phase 1 Re-check**: All principles remain satisfied at design level. Data model preserves ledger immutability, entities use domain methods for state transitions, projection engine is a pure domain service, project structure follows ABP layering conventions.

**Post-Phase 3 Re-check (2026-03-13)**: Two implementation violations found. Principle I fails due to hardcoded Arabic strings in Angular components and RealInvest UI remnants. Principle III partially implemented — idempotency key column exists on LedgerEntry but no HTTP middleware to enforce it on financial endpoints. All violations tracked in Phase 3b remediation tasks (T184–T188). Constitution check will return to PASS after Phase 3b completion.

**Analyze findings resolved (2026-03-08)**:
- **C1 (CRITICAL)**: KYC presigned URL / secure image serving added — see Security Architecture below and tasks T168, T168b.
- **C2 (HIGH)**: ABP audit logging expanded to cover all user-initiated sensitive actions, not just admin actions — see T162, T162b.
- **C3 (HIGH)**: T152 notification wiring split into per-domain-manager tasks (T152a–T152f) to comply with PR ≤ 400 LOC rule.
- **I1 (HIGH)**: Infrastructure project clarified — file storage implementations live in `MshNawy.EntityFrameworkCore/Infrastructure/` (no separate project needed).
- **I2 (MEDIUM)**: KYC "Under Review" state explicitly included in T036 domain methods.
- **I3 (MEDIUM)**: T118 split into T118a/T118b/T118c.
- **U3 (MEDIUM)**: Withdrawal integration test task added (T183).
- **U4 (MEDIUM)**: FR-030 (full upfront, no payment plan) test case added to T095 description.

**Analyze findings resolved (2026-03-08 — second pass)**:
- **I1 (CRITICAL)**: All `RealInvest:` error codes in api-contracts.md replaced with `MshNawy:NNNN` codes matching `MshNawyErrorCodes.cs`.
- **I3 (HIGH)**: KYC upload flow in api-contracts.md updated to two-step secure upload: `POST /kyc/upload` → file token, then `POST /kyc/submit` with tokens. Removed inline multipart file upload from submit endpoint.
- **U4 (HIGH)**: T168/T168b (IFileStorageService + KYC image API) moved from Phase 13 to Phase 3 as prerequisite for KYC upload.
- **U1 (HIGH)**: T034 updated — AppUser uses composition (separate aggregate root referencing ABP IdentityUser.Id via FK), not inheritance. All fields explicitly listed.
- **U2 (HIGH)**: T035 updated — OTP stored as SHA256 hash on AppUser entity, with expiration. Storage approach fully specified.
- **U3 (MEDIUM)**: T037 updated — JWT generation strategy documented: uses ABP's ITokenService/SignInManager after OTP verification.
- **C1 (HIGH)**: T000 marked done (source code rename complete). api-contracts.md error codes updated separately.
- **C3 (MEDIUM)**: ar.json expanded with Arabic translations for all error codes in MshNawyErrorCodes.cs.
- **C4 (MEDIUM)**: FluentValidation DI registration wired in MshNawyApplicationModule via `AddValidatorsFromAssembly`. Added `FluentValidation.DependencyInjectionExtensions` package.
- **I4 (MEDIUM)**: IPA address in api-contracts.md changed from `realinvest@instapay` to `mshnawy@instapay`.

**Analyze findings resolved (2026-03-13 — post-implementation Phase 0–3 analysis)**:
- **C1 (CRITICAL)**: Hardcoded Arabic strings in Angular components (onboarding-login, onboarding-kyc, kyc-status, error-state) violate Constitution §I. New task T184 added to replace with ABP localization service calls.
- **I1 (HIGH)**: "RealInvest" remnants in `angular/src/index.html` title and `app-shell.component.html` brand. New task T185 added to complete rename.
- **C2 (HIGH)**: CSS classes use `ri-` prefix and component selector is `ri-app-shell` — inconsistent with MshNawy rename. Addressed in T185.
- **U1 (MEDIUM)**: Auth and KYC guards are placeholder implementations not wired to actual services. New task T186 added to connect guards to AuthService/KycService.
- **U2 (MEDIUM)**: T040 (EF Core KYC migration) was done but unchecked. Marked as `[X]`.
- **C3 (MEDIUM)**: Idempotency key middleware not yet implemented despite FR-013. New task T187 added to Phase 2 (foundational) as prerequisite for Phase 4+ financial endpoints.
- **U3/I2 (MEDIUM)**: Refresh token in api-contracts.md and AuthResultDto but semantics unspecified. Deferred to post-MVP — refresh token field removed from MVP scope; MVP uses short-lived JWT only. api-contracts.md to be updated.
- **I3 (LOW)**: Default Arabic error message hardcoded in error-state component `@Input()`. Addressed in T184.

## Project Structure

### Documentation (this feature)

```text
specs/001-realinvest-mvp/
├── spec.md              # Feature specification
├── plan.md              # This file
├── research.md          # Phase 0: Technology decisions
├── data-model.md        # Phase 1: Entity definitions
├── quickstart.md        # Phase 1: Setup + run commands
├── contracts/
│   └── api-contracts.md # Phase 1: API endpoint contracts
└── tasks.md             # Phase 2: Task breakdown (via /speckit.tasks)
```

### Source Code (repository root)

```text
aspnet-core/
├── src/
│   ├── MshNawy.Domain.Shared/        # Enums, constants, error codes
│   ├── MshNawy.Domain/               # Entities, aggregates, domain services
│   │   ├── Identity/                 # User extensions, OTP, KYC state machine
│   │   ├── Wallet/                   # LedgerEntry, LedgerService, BalanceCalculator
│   │   ├── Deposits/                 # Deposit aggregate, DepositManager
│   │   ├── Withdrawals/              # Withdrawal aggregate, WithdrawalManager
│   │   ├── Offerings/                # Offering, FinancialModel, ProjectionEngine
│   │   ├── Orders/                   # InvestmentOrder, Installment, OrderManager
│   │   ├── Portfolio/                # Holding, StatementGenerator
│   │   ├── PropertySales/            # PropertySale, SaleVote, SaleDistribution, SaleManager
│   │   ├── Fees/                     # FeePolicy, FeeCalculator
│   │   ├── Support/                  # SupportTicket, messages, attachments
│   │   ├── Notifications/            # Notification entity, NotificationService
│   │   └── Shared/                   # IFileStorageService interface, shared value objects
│   ├── MshNawy.Application.Contracts/   # DTOs, service interfaces
│   ├── MshNawy.Application/              # Application services, AutoMapper
│   ├── MshNawy.EntityFrameworkCore/     # DbContext, migrations, repositories
│   │   └── Infrastructure/              # IFileStorageService implementations
│   │       └── FileStorage/             # LocalFileStorageService (MVP) → CloudFileStorageService (prod)
│   ├── MshNawy.HttpApi/                  # API controllers
│   ├── MshNawy.HttpApi.Host/             # Host startup, configuration
│   └── MshNawy.DbMigrator/              # Migration runner
└── test/
    ├── MshNawy.Domain.Tests/             # Unit: fees, projections, state machines
    ├── MshNawy.Application.Tests/        # Integration: ledger, idempotency
    └── MshNawy.HttpApi.Tests/            # API contract tests

angular/
├── src/app/
│   ├── shared/            # Components, pipes, guards, models (all with Storybook)
│   ├── onboarding/        # OTP + KYC flows
│   ├── wallet/            # Balances, deposits, withdrawals
│   ├── offerings/         # Browse, detail, projections
│   ├── subscription/      # Knowledge check, order flow
│   ├── portfolio/         # Holdings, statements
│   ├── property-sales/    # Sale vote, property sale status, proceeds
│   ├── support/           # Ticket management
│   ├── notifications/     # Notification center
│   ├── admin/             # Admin panel (lazy-loaded module)
│   └── mock/              # MSW mock API layer
│       ├── data/          # Deterministic seed data files
│       └── handlers/      # MSW request handlers per domain
├── .storybook/
└── e2e/
```

**Structure Decision**: Standard ABP multi-layer web application with Angular frontend. Single ABP module with domain-driven folder organization per bounded context. Admin panel as a lazy-loaded Angular module sharing the same backend API with admin-specific permission-protected endpoints. `IFileStorageService` implementations reside in `MshNawy.EntityFrameworkCore/Infrastructure/FileStorage/` — infrastructure co-located with the EF Core layer, no separate Infrastructure project needed at MVP scale.

## Security Architecture

### KYC Image Handling (Constitution §Security — MUST)

KYC images (national ID front/back, selfie) must never be publicly accessible:

1. **Upload flow**: Client receives a short-lived upload token from `POST /api/app/kyc/upload-url`, then streams the file to `POST /api/app/kyc/upload` with the token. The file is stored outside `wwwroot`.
2. **Storage (MVP)**: `LocalFileStorageService` writes files to a configured directory that is not web-accessible. Files are served exclusively through authenticated API endpoints that validate JWT + file ownership.
3. **Retrieval**: `GET /api/app/kyc/image/{fileToken}` — requires valid JWT + ownership check. Returns the file stream; no permanent public URL is returned.
4. **Production**: Replace `LocalFileStorageService` with `AzureBlobFileStorageService` by updating DI registration in `MshNawyHttpApiHostModule`. Presigned short-lived URLs are generated at the application service layer. Zero application-layer code changes required.
5. **Audit**: All KYC image access events are captured by ABP audit logging.

### Audit Logging Coverage (Constitution §Security — MUST)

ABP `AbpAuditingOptions` must cover **all** sensitive actions — both user and admin initiated:

| Category | Events Logged |
|----------|--------------|
| Authentication | Login, logout, OTP request, OTP verification, OTP rate-limit exceeded |
| KYC | Submission, status transition (each), image upload, image access |
| Financial (user) | Deposit created, deposit status change, withdrawal created, withdrawal status change, order created/reserved/settled/failed, installment paid/overdue/flagged |
| Exit | Sale vote cast, sale vote withdrawn, sale proceeds received |
| Admin | KYC approve/reject, deposit approve/reject, withdrawal approve/reject, order settle/fail, property sale initiated/settled, fee policy changed, offering created/updated |
| Security | Failed login attempts, OTP lockout triggered |

## Run Commands

See [quickstart.md](./quickstart.md) for full setup and first-time configuration.

### Quick Reference

```bash
# ── Backend ──────────────────────────────────────────────────
# Run database migrations (first time / after new migration)
cd aspnet-core/src/MshNawy.DbMigrator && dotnet run

# Start API server
cd aspnet-core/src/MshNawy.HttpApi.Host && dotnet run
# API: https://localhost:44300  |  Swagger: https://localhost:44300/swagger

# ── Frontend ─────────────────────────────────────────────────
# Mock mode (no backend needed — MSW intercepts all requests)
cd angular && ng serve
# App: http://localhost:4200

# Real API mode (backend must be running)
# Set mockEnabled: false in angular/src/environments/environment.ts
cd angular && ng serve

# ── Testing ──────────────────────────────────────────────────
cd aspnet-core && dotnet test                          # all backend tests
cd angular && ng test                                  # unit tests (Jest)
cd angular && npm run storybook                        # component catalog
cd angular && npx playwright test                      # E2E tests

# ── Build ────────────────────────────────────────────────────
cd angular && ng build --configuration=production      # ≤250KB gzip budget enforced
cd aspnet-core && dotnet build                         # zero-warning strict mode
```

## Complexity Tracking

> Open exceptions require project-owner approval and are tracked below.

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| StatementGenerator in Domain layer generates HTML | Keeps statement logic co-located with financial domain knowledge (holdings, fees, projections). | Moving to Application layer would split financial calculation logic from rendering. The HTML template is simple enough for MVP. Extracting an `IStatementRenderer` interface is a post-MVP refactor if PDF support is added. |
| LocalFileStorageService (MVP) instead of presigned cloud URLs | MVP targets 100 users with manual KYC review. Local storage + authenticated API endpoint is constitution-compliant: no public URLs, audit logged, owned by authed user. | Cloud storage (Azure Blob/S3 with presigned URLs) is the production target. The `IFileStorageService` abstraction makes the swap zero-application-code-change. Exception is time-bounded to MVP only. |
| T152a–T152f split into 6 tasks | Each domain manager notification wiring (Deposit, Withdrawal, Order, Sale, Support, KYC) is a separate PR to comply with the ≤ 400 LOC constitution rule. | A single task touching all 6 managers simultaneously guarantees a PR > 400 LOC. |

## Project Rename: RealInvest → MshNawy

The ABP solution was scaffolded as `RealInvest` (`abp new RealInvest`). All spec documents now use `MshNawy`. Task **T000** in tasks.md covers the full codebase rename before Phase 3 implementation begins.

**Rename scope**:
- `.sln` and `.csproj` files: `RealInvest.*` → `MshNawy.*`
- All `namespace RealInvest.*` → `namespace MshNawy.*`
- All `using RealInvest.*` → `using MshNawy.*`
- Connection string database name: `Database=RealInvest` → `Database=MshNawy`
- Angular `package.json` name: `"realinvest"` → `"mshn-nawy"`
- ABP localization resource key prefix: `RealInvest:` → `MshNawy:`

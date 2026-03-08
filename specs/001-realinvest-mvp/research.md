# Research: MshNawy (مش ناوي) MVP

**Branch**: `001-realinvest-mvp` | **Date**: 2026-02-28 | **Updated**: 2026-03-08

## R1: ABP Module Structure

**Decision**: Single ABP module (`RealInvest`) with domain-driven folder organization within each layer, not separate ABP modules per bounded context.

**Rationale**: MVP scope is a single product with tightly coupled domains (wallet ↔ ledger ↔ offerings ↔ orders). Separate ABP modules add significant boilerplate (separate DbContexts, module dependencies, permission groups) without benefit at this scale. Domain boundaries are enforced via folder structure and aggregate boundaries instead.

**Alternatives considered**:
- Multi-module (Wallet module, Offering module, etc.) — rejected: excessive ceremony for MVP, cross-module transactions become complex (ledger entries span domains).

## R2: Double-Entry Ledger Implementation

**Decision**: Custom ledger implementation as a core Domain service within ABP. Ledger entries are a single `LedgerEntry` entity with debit/credit account references. Accounts are identified by type + owner (e.g., `UserAvailable:userId`, `PlatformFees`, `SettlementPending`).

**Rationale**: No suitable open-source .NET double-entry ledger library exists that integrates cleanly with ABP's entity/repository model. The ledger is core business logic that must be fully controlled. The account model (string-based account identifiers with type prefix) is simple enough for MVP while supporting future account hierarchy.

**Alternatives considered**:
- Use a separate ledger microservice — rejected: adds distributed transaction complexity for MVP.
- Use an off-the-shelf accounting package — rejected: none integrate with ABP DDD patterns.

**Account types for MVP**:
- `User:Available:{userId}` — user's available balance
- `User:Reserved:{userId}` — user's reserved balance
- `User:Invested:{userId}` — user's invested balance
- `User:PendingWithdrawal:{userId}` — user's pending withdrawal
- `Platform:Settlement` — settlement clearing account
- `Platform:EntryFees` — entry fee revenue
- `Platform:PaymentFees` — payment fee revenue
- `Platform:ExitBrokerage` — exit brokerage revenue
- `Platform:ExitProfit` — exit platform profit revenue
- `Platform:WithdrawalFees` — withdrawal fee revenue (Vodafone Cash 5 EGP)

## R3: Projection Engine Design

**Decision**: Pure domain service (`ProjectionEngine`) that takes a `ProjectionInput` value object (property price, share count, payment plan, rent, occupancy, appreciation, exit date, fee policy) and returns a `ProjectionResult` value object (conservative/base/optimistic scenarios with annual returns, distribution schedule, exit values, fee impact). No external dependencies. Purely computational.

**Rationale**: The projection engine is the core differentiator. It must be deterministic, testable in isolation, and have zero side effects. A pure function approach enables exhaustive unit testing and ensures reproducibility (SC-006).

**Alternatives considered**:
- Spreadsheet-based calculation with import — rejected: not programmable, hard to version.
- External calculation service — rejected: unnecessary network dependency for pure math.

**Scenario parameters**:
| Parameter | Conservative | Base | Optimistic |
|-----------|-------------|------|------------|
| Occupancy rate | 70% | 85% | 95% |
| Appreciation rate | 3% p.a. | 7% p.a. | 12% p.a. |
| Rent growth | 0% p.a. | 5% p.a. | 10% p.a. |

## R4: Mock API Strategy

**Decision**: MSW (Mock Service Worker) for the Angular frontend. MSW intercepts fetch/XHR at the network level, allowing the Angular app to use real `HttpClient` calls with zero code changes when switching to the real backend.

**Rationale**: MSW provides the most realistic mock experience — the Angular app doesn't know it's using mocks. This aligns with Constitution Principle VI (frontend-first) requirement that "no frontend code changes should be needed at integration time." It also enables Storybook stories to use the same mock handlers.

**Alternatives considered**:
- `json-server` — rejected: requires running a separate process, doesn't support complex state transitions.
- Angular `in-memory-web-api` — rejected: intercepts at the Angular HttpClient level, less realistic, harder to share with Storybook.

## R5: File Upload Strategy (KYC, Deposit Proof)

**Decision**: Local file system storage for MVP with an abstraction layer (`IFileStorageService`) that can be swapped to Azure Blob/S3 in production. Files stored in a configured directory, served via authenticated API endpoints (not direct file access).

**Rationale**: MVP targets 100 concurrent users with manual KYC review. Cloud storage adds configuration complexity without benefit at this scale. The abstraction ensures zero code changes when migrating to cloud storage.

**Alternatives considered**:
- Direct cloud storage from day 1 — rejected: adds cloud provider dependency and configuration overhead for MVP.
- Base64 in database — rejected: bloats database, poor performance for images.

## R6: Installment Scheduler

**Decision**: ABP Background Jobs (`IBackgroundJob`) with a recurring job that runs daily, checks for due installments, and processes them. Uses ABP's built-in job infrastructure (Hangfire or default implementation).

**Rationale**: ABP provides built-in background job support. Daily processing is sufficient for monthly installments. The job is idempotent (processes only installments in `Due` status, skips already-processed ones).

**Alternatives considered**:
- Real-time processing on exact due date/time — rejected: over-engineering for monthly cycles, adds clock synchronization concerns.
- Manual back-office trigger — rejected: defeats the purpose of automated installments.

## R7: Admin Panel Approach

**Decision**: Separate Angular application (or lazy-loaded module within the same Angular app) using ABP's built-in admin theme. Simple list + detail CRUD screens using ABP's CRUD application service base classes. Shares the same backend API but with admin-specific endpoints protected by ABP permissions.

**Rationale**: ABP provides extensive admin infrastructure (permission management, audit logs, user management). Leveraging this reduces development effort significantly. The admin panel does not need the same level of UI polish as the investor-facing app.

**Alternatives considered**:
- Completely separate admin backend — rejected: duplicates entity access and increases maintenance.
- No admin UI, API-only — rejected: clarification confirmed full admin panel in MVP scope.

## R8: Project Name — MshNawy (مش ناوي)

**Decision**: The product name is **MshNawy (مش ناوي)** — meaning "Not Nawy" in Egyptian Arabic, positioning the platform as a distinctive local alternative. All user-facing strings, documentation, and code namespaces use this name. ABP solution namespaces: `MshNawy.*`. Angular package name: `mshn-nawy`.

**Rationale**: The name is intentionally Egyptian-market-specific, culturally resonant, and unambiguous in Arabic. It signals independence from competing platforms without referencing them explicitly in the UI (constitution §IV prohibits competing brand references in UI).

**Alternatives considered**:
- Keeping "RealInvest" — rejected: generic English name, not distinctive in Egyptian market, does not reflect Arabic-first identity.
- "MashaaNawy" (transliteration variant) — rejected: less readable in Latin script for developer tooling.

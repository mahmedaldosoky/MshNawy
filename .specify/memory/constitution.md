<!--
SYNC IMPACT REPORT
==================
Version change: 2.5.0 → 2.9.0 (MINOR — strengthened naming, file, and
validation ownership rules in Principles VII and VIII)

Modified principles:
  - VII. Code Quality: strengthened "DTO naming" rule to explicitly forbid
    `Input` suffix — all must end with `RequestDto`. Replaced "One DTO per
    file" with "One class per file" — inline classes in controllers or other
    files are forbidden.
  - VIII. Interface-Based Dependencies: added domain service interface rule,
    one-handler-per-file rule, 1:1 validator-per-handler rule (validators
    own all input validation, handlers MUST NOT duplicate). Strengthened
    DTO-is-the-request rule. Updated type location guide.

Added sections:
  - None

Removed sections:
  - None

Templates reviewed:
  - .specify/templates/plan-template.md       ✅ Compatible
  - .specify/templates/spec-template.md       ✅ Compatible
  - .specify/templates/tasks-template.md      ✅ Compatible

Deferred TODOs:
  - None. All placeholders resolved.
-->

# MshNawy Constitution

## Core Principles

### I. Arabic-Only & Egyptian Market (NON-NEGOTIABLE)

MshNawy is an Arabic-first product targeting Egyptian investors exclusively.
Every user-facing surface MUST comply with the following rules:

- **Language**: The product UI MUST be Arabic-only. No English strings, labels,
  tooltips, error messages, or placeholder text are permitted in any user-facing
  screen. Internal developer tools, logs, and admin dashboards are exempt.
- **Direction**: All layouts MUST be RTL (right-to-left). The Angular application
  MUST set `dir="rtl"` on the root `<html>` element. CSS logical properties
  (`margin-inline-start`, `padding-inline-end`) MUST be used instead of physical
  directional properties (`margin-left`, `padding-right`).
- **Locale**: All numbers, dates, and currency values MUST render in Arabic locale
  (`ar-EG`). Currency MUST display as Egyptian Pounds (EGP / ج.م.) with Arabic
  numerals. No dollar signs, no Latin-numeral formatting.
- **Eligibility gate**: Users MUST NOT access any investing, depositing, or
  portfolio feature until they have completed:
  1. Egyptian phone number verification via OTP.
  2. Egyptian national ID upload and validation.
  This gate MUST be enforced at both the UI routing layer (Angular guards) and the
  API authorization layer (ABP permission/policy). Bypassing is a critical security
  defect.
- **Localization infrastructure**: All UI strings MUST be stored in ABP localization
  resource files (`ar.json`). Hard-coded Arabic strings in Angular templates or
  component code are forbidden — use `L["Key"]` on the backend and localization
  pipes/services on the frontend.

**Rationale**: Regulatory compliance and user trust require a fully localized,
Egyptian-market-only product. Mixed-language UIs or non-EGP currencies would
confuse users and violate Egyptian fintech expectations.

### II. Financial Integrity — Double-Entry Ledger (NON-NEGOTIABLE)

All monetary movements in MshNawy MUST be recorded via a double-entry ledger.
No balance may change without a corresponding ledger entry.

- **Ledger-first rule**: Every operation that changes a user's balance, an escrow
  account, a fee account, or a platform revenue account MUST create ledger entries
  (debit + credit) BEFORE updating any derived balance or state.
- **Immutability**: Approved/posted ledger entries are immutable. They MUST NOT be
  updated or deleted. Corrections MUST be made via compensating (reversal) entries
  that reference the original entry ID.
- **Audit trail**: Every ledger entry MUST record: timestamp (UTC), actor ID,
  entry type, debit account, credit account, amount (in minor units — piasters),
  idempotency key, and a human-readable description.
- **Balance derivation**: Account balances MUST be derivable from the sum of ledger
  entries at any point in time. Cached/materialized balances are permitted for
  read performance but MUST be reconcilable against the ledger. Any discrepancy
  is a P0 incident.
- **Minor units**: All monetary values in the backend MUST be stored and computed
  in piasters (1 EGP = 100 piasters) as `long` / `Int64` to avoid floating-point
  errors. Conversion to display format (EGP with decimal) happens exclusively in
  the presentation layer.

**Rationale**: Real estate fractional ownership involves real money. A single
ledger inconsistency can cause financial loss, regulatory violation, or legal
dispute. Double-entry is the industry standard for financial correctness.

### III. Atomic & Idempotent Financial Operations (NON-NEGOTIABLE)

Every financial flow MUST be both atomic and idempotent.

- **Atomicity**: Deposits, withdrawals, share purchases, payment plan
  installments, fee deductions, and settlement actions MUST execute within a
  single database transaction. If any step fails, the entire operation MUST
  roll back — no partial state.
- **Idempotency keys**: Every mutating financial API endpoint MUST accept a
  client-generated idempotency key (UUID). The server MUST store the key and
  return the original response for duplicate submissions within a 24-hour
  window. Duplicate detection MUST use a unique constraint on the idempotency
  key column.
- **State machines**: Financial entities with lifecycle states (e.g., Order:
  `Pending → Confirmed → Settled → Cancelled`; Withdrawal: `Requested →
  Approved → Processed → Failed`) MUST be modeled as explicit state machines
  with guarded transitions. Direct status column updates via SQL or ORM are
  forbidden; transitions MUST go through domain methods that validate
  preconditions and emit domain events.
- **Concurrency control**: Optimistic concurrency (`ConcurrencyStamp` in ABP)
  MUST be enabled on all financial aggregate roots. Stale-write detection is
  mandatory.

**Rationale**: Network retries, user double-taps, and infrastructure failures
are inevitable. Without idempotency and atomicity, users can be double-charged
or funds can be lost.

### IV. Configurable Fee Policy (NON-NEGOTIABLE)

MshNawy fees MUST follow the business model and MUST be implemented as
configurable policy rules — never as hard-coded constants.

- **Current fee schedule** (source: Nawy Shares FAQ — keep reference in
  internal docs only, never in UI):
  - **Entry fee**: 1% of share value, collected with the down payment(s).
  - **Payment fee**: 3% of each payment installment value.
  - **Exit fee**: 5% of unit resale price, split as:
    - 2.5% resale brokerage.
    - 2.5% platform profit.
- **Configurability**: Fee percentages, thresholds, and split ratios MUST be
  stored in a `FeePolicy` configuration entity (database-backed, not
  `appsettings.json`). Changes to fee policy MUST be auditable (who changed,
  when, previous values). Fee policy MUST support effective-date ranges so
  existing investments retain their original fee terms.
- **Calculation transparency**: Every fee calculation MUST be reproducible:
  given the same inputs (share value, payment amount, resale price) and the
  same fee policy version, the output MUST be identical. Fee breakdowns MUST
  be stored on the transaction record — not recomputed on read.
- **Display rule**: Fee breakdowns MUST be shown to the user before any
  confirmation step (deposit, purchase, exit request). No hidden fees.
- **No Nawy references in UI**: The Nawy Shares brand, FAQ, or any source
  references MUST NOT appear in the product UI or user-facing content. These
  references are for internal documentation only.

**Rationale**: Fees are the core revenue mechanism. Hard-coded fees are a
maintenance trap and a compliance risk when regulations or business strategy
change.

### V. Dynamic Financial Projections — No Static Numbers

Every investment opportunity displayed to users MUST be backed by a
computational model with explicit inputs and generated outputs. Static or
hard-coded ROI percentages in the UI are forbidden.

- **Projection engine**: A projection engine MUST exist in the Domain layer
  that computes expected returns from underlying property cashflow inputs:
  - Rental income (monthly/annual estimates).
  - Occupancy rate assumptions.
  - Property appreciation rate assumptions.
  - Fee schedule (per Principle IV).
  - Payment plan structure (down payment, installments, balloon).
- **Investment card contract**: Every investment card rendered in the UI MUST
  be backed by a model containing:
  - **Inputs**: All assumptions listed above, with source attribution.
  - **Generated outputs**: Projected timeline of distributions, expected
    exit value range (optimistic / base / pessimistic), net IRR estimate.
  - **Sensitivity**: If inputs change (e.g., occupancy drops 10%), the
    outputs MUST recompute. No cached-forever projections.
- **Disclaimer**: Projections MUST display a visible disclaimer that results
  are estimates and not guaranteed.
- **No vanity metrics**: The UI MUST NOT display isolated "X% annual return"
  numbers without the underlying assumptions being accessible to the user
  (expandable detail or linked projection page).

**Rationale**: Static ROI claims are misleading and potentially illegal under
Egyptian financial regulations. Dynamic projections backed by transparent
inputs build trust and regulatory defensibility.

### VI. Frontend-First Delivery Strategy

MshNawy follows a frontend-first development methodology. The UI is built
and validated before backend implementation begins.

- **Build order**: For every feature:
  1. Define the API contract (request/response DTOs, endpoints, error codes).
  2. Build the Angular frontend against mock API services and deterministic
     mock data files.
  3. Validate the complete UI flow (including loading, empty, and error
     states) using Storybook and E2E tests against mocks.
  4. Build the .NET/ABP backend to match the exact same API contracts.
  5. Integrate by swapping the Angular `HttpClient` base URL from mock
     server to real API. No frontend code changes should be needed at
     integration time.
- **Mock data**: Mock data files MUST use deterministic seeds so that
  screenshots, E2E tests, and demos are reproducible. Mocks MUST cover:
  happy path data, empty states, error responses, edge cases (max values,
  Arabic long strings, zero balances).
- **Contract alignment**: The mock API response shapes MUST exactly match
  the Application.Contracts DTOs. Any drift between mock and real API is a
  blocking defect.
- **Storybook**: All reusable UI components MUST have Storybook stories
  demonstrating: default state, loading state, empty state, error state,
  RTL layout, and mobile viewport.

**Rationale**: Frontend-first de-risks the most subjective part of the product
(UX) before investing in backend complexity. It also enables parallel
development: frontend and backend teams can work simultaneously once contracts
are agreed.

### VII. Code Quality & ABP Compliance

All code MUST follow established quality standards and ABP Framework
architectural conventions.

- **ABP layering**: The multi-layer ABP structure (Domain, Domain.Shared,
  Application, Application.Contracts, HttpApi, HttpApi.Host, EntityFramework)
  MUST be preserved. Cross-layer dependency violations are blocking defects.
- **DDD enforcement**: Aggregates MUST enforce their own invariants via
  domain methods. No entity property is publicly settable without a guarded
  method. Domain events MUST be used for cross-aggregate side-effects.
- **DTOs & mapping**: All data crossing the Application/HttpApi boundary MUST
  be a DTO. AutoMapper profiles MUST be defined in the Application layer.
  Entity-to-DTO mapping MUST happen in query classes (see Principle VIII).
- **DTO naming**: All DTOs MUST end with `RequestDto` (for inputs/commands)
  or `ResponseDto` (for outputs/results). Examples: `KycSubmitRequestDto`,
  `KycStatusResponseDto`, `KycUploadResponseDto`. Classes named with
  `Input` suffix (e.g., `SendOtpInput`, `KycReviewDecisionInput`) are
  forbidden — rename to `RequestDto`. This applies to ALL layers including
  HttpApi-layer form DTOs (e.g., `KycUploadFormRequestDto`).
- **One DTO per file**: Each file MUST contain at most one `RequestDto` or
  one `ResponseDto`. The file MUST be named after the DTO it contains
  (e.g., `SendOtpRequestDto.cs`, `send-otp-request.dto.ts`). The only
  exception: a helper object (enum, nested type, or auxiliary interface)
  that is used exclusively by that DTO may share the same file. If the
  helper is referenced by any other type, it MUST be extracted to its own
  file. Inline classes/interfaces inside controller files, app service
  files, component files, or any other non-DTO file are forbidden — every
  DTO MUST be in a dedicated file. This rule applies to both backend (C#)
  and frontend (TypeScript) codebases.
- **Naming**: Names MUST be self-documenting. No abbreviations except
  universally accepted acronyms (`Id`, `Dto`, `Api`, `Otp`, `Kyc`, `Egp`).
- **Canonical type definitions**: Shared enums and union types (e.g.,
  `KycStatus`, `KycReviewDecision`) MUST be defined once in
  `angular/src/app/shared/models/` and imported everywhere they are needed.
  Inline re-declaration of the same string-literal union in a component or
  service is forbidden. If a type is used in more than one file it MUST live
  in a shared models file.
- **Dead code**: No commented-out code, unused imports, or orphaned files in
  the `main` branch.
- **PR size**: A single PR MUST NOT change more than 400 lines of production
  code (excluding generated files, migrations, and test files).
- **Exception handling**: Business rule violations MUST throw typed ABP
  exceptions (`BusinessException` with error codes). Generic `throw new
  Exception()` in Domain or Application layers is forbidden.
- **Authorization**: Permissions MUST be defined in `[Module]Permissions.cs`
  and enforced via attributes or `IPermissionChecker`. Hard-coded role names
  in business logic are forbidden.

**Rationale**: ABP provides tested infrastructure. Following its conventions
prevents hidden complexity and ensures upgradability.

### VIII. Interface-Based Dependencies, CQRS Layers, MediatR & Repository Pattern (NON-NEGOTIABLE)

All cross-layer dependencies MUST go through interfaces — never concrete classes.
All database query and persistence logic MUST live in repository implementations,
not in application services or controllers. All entity-to-DTO mapping MUST live
in query classes using AutoMapper — never manual mapping in app services.
Application services MUST be thin coordinators that extract user context and
delegate reads to query classes and writes to MediatR via `IMediator.Send()`.

**Dependency Chain** (read path):

```
Controller → IAppService → AppService → IQuery → Query → IRepo → Repo
```

**Dependency Chain** (write path):

```
Controller → IAppService → AppService → IMediator → CommandHandler → IRepo → Repo
```

- **Controllers → App Service Interfaces**: HttpApi controllers MUST inject
  application service interfaces (e.g., `IAuthAppService`, `IKycAppService`)
  defined in `Application.Contracts`. Controllers MUST NOT reference concrete
  `ApplicationService` classes or the `Application` project directly.
- **App Services as thin coordinators**: Application services MUST be thin
  wrappers that only extract user context (`CurrentUser.Id`, `Clock.Now`) and
  delegate to query interfaces (reads) or `IMediator.Send()` (writes).
  Application services MUST NOT contain business logic, domain operations,
  DTO mapping, or direct repository calls.
- **App Services → Query Interfaces (reads)**: For read operations that return
  DTOs, application services MUST delegate to query interfaces (e.g.,
  `IAppUserQuery`) defined in `Application.Contracts`. Query implementations
  live in the `Application` project, call repository interfaces, and use
  AutoMapper (`IObjectMapper`) for all entity-to-DTO mapping.
- **App Services → MediatR (writes)**: For write/mutation operations,
  application services MUST dispatch request DTOs via `IMediator.Send()`.
  Request DTOs (e.g., `KycSubmitRequestDto`, `SendOtpRequestDto`) implement
  `IRequest<TResult>` (or `IRequest` for void) and are defined in
  `Application.Contracts`. There are NO separate command/record files —
  the request DTO IS the MediatR request. Creating intermediate command
  classes (`SendOtpCommand`, `VerifyOtpCommand`, etc.) that duplicate DTO
  properties is forbidden. App services MUST forward the request DTO
  directly: `return await mediator.Send(request);` — never construct a
  separate object. Command handlers have NO interface — they implement
  `IRequestHandler<TRequestDto, TResult>` from MediatR and live in the
  `Application` project.
- **One handler per file**: Each command handler MUST live in its own file
  named after the handler class (e.g., `SendOtpCommandHandler.cs`,
  `SubmitKycCommandHandler.cs`). A file MUST NOT contain more than one
  handler class. This is a blocking defect in code review.
- **One validator per handler (1:1)**: Every command handler MUST have a
  corresponding FluentValidation validator class for its request DTO.
  The validator file MUST be named `{RequestDto}Validator.cs` (e.g.,
  `SendOtpRequestDtoValidator.cs`) and live in the same folder as its
  handler. Validators handle all input validation (format checks, required
  fields, range constraints). Command handlers MUST NOT duplicate
  validation rules that belong in the validator. Validators are
  auto-discovered via `AddValidatorsFromAssembly` in
  `MshNawyApplicationModule`.
- **Domain Services → Interfaces**: Every domain service class (e.g.,
  `OtpService`, `LedgerService`, `BalanceCalculator`, `FeeCalculator`) MUST
  have a corresponding interface (e.g., `IOtpService`, `ILedgerService`,
  `IBalanceCalculator`, `IFeeCalculator`) defined in the same Domain project
  folder. All consumers — command handlers, query classes, other domain
  services — MUST inject the interface, never the concrete class. Domain
  service interfaces are registered in `MshNawyDomainModule.ConfigureServices`
  (e.g., `context.Services.AddTransient<IOtpService, OtpService>()`).
- **Command Handlers → Repository Interfaces**: Command handler implementations
  MUST inject custom repository interfaces (e.g., `IAppUserRepository`) defined
  in the `Domain` project. Custom repository interfaces extend ABP's
  `IRepository<TEntity, TKey>`. Concrete EF Core repository implementations
  live in `EntityFrameworkCore/Repositories/`.
- **Query Classes → Repository Interfaces**: Query implementations MUST call
  repository interface methods to retrieve entities, then map to DTOs using
  AutoMapper. Query classes MUST NOT contain business logic — only data
  retrieval and mapping.
- **AutoMapper profiles**: All entity-to-DTO mappings MUST be defined in
  AutoMapper `Profile` classes in the `Application` project (e.g.,
  `MshNawyApplicationAutoMapperProfile`). Manual DTO construction
  (`new SomeDto { ... }`) from entity properties in app services is forbidden.
  DTOs composed from multiple non-entity sources (e.g., JWT tokens,
  computed values) are exempt from this rule and may be constructed manually
  in command handlers.
- **Command/Query handlers → Infrastructure Interfaces**: Infrastructure
  abstractions (e.g., `IFileStorageService`, `IOtpSender`, `IJwtTokenService`)
  MUST be defined in `Domain.Shared` or `Application.Contracts`.
  Implementations live in `EntityFrameworkCore/Infrastructure/` or
  `Application/`. Command handlers and query classes inject these directly.
- **HttpApi project references**: The `HttpApi` project MUST reference only
  `Application.Contracts` — never `Application` or `Domain` directly. All
  types needed by controllers (DTOs, interfaces) MUST be defined in
  `Application.Contracts`.
- **DB logic in repositories only**: All Entity Framework queries, LINQ
  expressions against `DbSet`, raw SQL, stored procedure calls, and any
  database-specific logic MUST reside in repository implementations inside
  the `EntityFrameworkCore` project. Command handlers, query classes, and
  application services MUST NOT call `GetQueryableAsync()`, write LINQ-to-SQL
  expressions, or use `AsyncExecuter` directly. Instead, domain-specific
  query methods MUST be defined on the repository interface and implemented
  in the EF Core repository class.
- **No leaking EF abstractions**: Command handlers, query classes, and
  application services MUST NOT depend on `IQueryable<T>`, `DbContext`,
  `DbSet<T>`, or any EF Core type. If a query needs a filtered or paginated
  result, the repository interface MUST expose a method with
  domain-meaningful parameters.

**Interface & Type Location Guide**:

| Type | Defined In | Implemented In |
|------|-----------|----------------|
| `IAuthAppService` | Application.Contracts | Application |
| `IKycAppService` | Application.Contracts | Application |
| `IKycReviewAppService` | Application.Contracts | Application |
| `IAppUserQuery` | Application.Contracts | Application |
| `SendOtpRequestDto`, `VerifyOtpRequestDto` | Application.Contracts | — (IRequest DTOs) |
| `KycSubmitRequestDto`, `KycUploadRequestDto` | Application.Contracts | — (IRequest DTOs) |
| `MoveToUnderReviewRequestDto`, `ReviewKycRequestDto` | Application.Contracts | — (IRequest DTOs) |
| `SendOtpCommandHandler`, etc. | — | Application (IRequestHandler) |
| `IOtpService` | Domain | Domain |
| `ILedgerService` | Domain | Domain |
| `IBalanceCalculator` | Domain | Domain |
| `IFeeCalculator` | Domain | Domain |
| `IAppUserRepository` | Domain | EntityFrameworkCore |
| `IFileStorageService` | Domain.Shared | EntityFrameworkCore/Infrastructure |
| `IOtpSender` | Application | Application |
| `IJwtTokenService` | Application | Application |

**Registration**:
- App service interfaces are auto-registered by ABP when the service class
  implements the interface and extends `ApplicationService`.
- MediatR is registered via `context.Services.AddMediatR(cfg =>
  cfg.RegisterServicesFromAssembly(typeof(MshNawyApplicationModule).Assembly))`
  in `MshNawyApplicationModule.ConfigureServices`. This auto-discovers all
  `IRequestHandler<,>` implementations.
- Command handlers also implement `ITransientDependency` for ABP DI compatibility.
- Query classes are registered via `ITransientDependency` marker interface.
- Domain services are registered manually in `MshNawyDomainModule.ConfigureServices`
  via `context.Services.AddTransient<IService, Service>()`. Every domain service
  MUST be registered against its interface — never as a concrete type alone.
- Custom repository interfaces are registered via
  `options.AddRepository<TEntity, TRepository>()` in the EF Core module.
- Infrastructure services are registered manually in module `ConfigureServices`.

**NuGet packages**:
- `MediatR` (v12.4.1) in `Application.csproj`
- `MediatR.Contracts` (v2.0.1) in `Application.Contracts.csproj`

**Rationale**: Dependency inversion enables testability (mock any dependency),
enforces separation of concerns, and prevents the application layer from being
coupled to EF Core internals. MediatR decouples app services from command
handler implementations — app services only know about command records (defined
in Contracts), not handler classes. Keeping DB logic in repositories ensures
that query changes don't require touching application services, and that all
data access is centralized and optimizable.

### IX. SOLID Principles (NON-NEGOTIABLE)

All backend code MUST adhere to the five SOLID principles. Violations are
blocking defects during code review.

- **S — Single Responsibility Principle (SRP)**: Every class MUST have exactly
  one reason to change. God classes and God services are forbidden.
  - Application services MUST only coordinate (extract context, delegate).
    They MUST NOT contain business logic, validation, mapping, AND
    orchestration in the same class.
  - Command handlers MUST handle exactly one command. A handler that processes
    multiple unrelated commands MUST be split.
  - Domain entities MUST encapsulate their own invariants but MUST NOT
    perform infrastructure concerns (sending emails, file I/O, logging).
  - Angular components MUST NOT exceed ~200 lines. If a component handles
    form logic, API calls, AND complex rendering, extract services or child
    components.
  - **Test**: If describing what a class does requires "and" (e.g., "validates
    input AND saves to DB AND sends notifications"), it violates SRP.

- **O — Open/Closed Principle (OCP)**: Classes MUST be open for extension but
  closed for modification.
  - Fee calculation MUST use the `FeePolicy` entity pattern (Principle IV)
    so that new fee types can be added without modifying `FeeCalculator`.
  - State machine transitions MUST be defined declaratively (transition
    tables or strategy objects), not via `if/else` chains that require
    editing when new states are added.
  - New MediatR command handlers extend the system without modifying
    existing application services — this is OCP by design.
  - Angular components MUST accept configuration via `@Input()` properties
    rather than hard-coding behavior. Prefer composition over conditional
    template branches.
  - **Test**: Adding a new feature (fee type, state, command) MUST NOT
    require modifying existing, tested classes.

- **L — Liskov Substitution Principle (LSP)**: Subtypes MUST be substitutable
  for their base types without altering program correctness.
  - All ABP repository implementations (e.g., `EfCoreAppUserRepository`)
    MUST honor the contract defined by their interface
    (`IAppUserRepository`). Throwing `NotImplementedException` for inherited
    methods is forbidden.
  - Custom exceptions MUST extend `BusinessException` (ABP) and MUST NOT
    change the exception-handling semantics (e.g., a subclass MUST NOT
    silently swallow errors that the base class would propagate).
  - When overriding ABP base class methods (e.g., `ApplicationService`,
    `AuditedEntity`), the override MUST preserve base class postconditions
    (e.g., audit fields MUST still be populated).
  - **Test**: Swapping a concrete implementation for its interface in a
    unit test MUST NOT cause test failures beyond the scope of the mock.

- **I — Interface Segregation Principle (ISP)**: Clients MUST NOT be forced
  to depend on interfaces they do not use.
  - Application service interfaces MUST be scoped per feature/aggregate
    (e.g., `IKycAppService`, `IAuthAppService`), not bundled into a single
    `IUserService` covering KYC, auth, profile, and wallet.
  - Repository interfaces MUST expose only the methods that their consumers
    actually call. If only `GetByIdAsync` and `InsertAsync` are needed,
    the custom interface MUST NOT expose `GetListAsync`, `UpdateAsync`,
    `DeleteAsync` unless they are used.
  - Query interfaces (e.g., `IAppUserQuery`) MUST be separate from command
    dispatch — never combined into a single read-write interface.
  - Infrastructure interfaces (e.g., `IFileStorageService`) MUST be
    single-purpose. An interface that handles file upload AND email sending
    MUST be split.
  - **Test**: If a mock implementation must stub methods that the test
    never calls, the interface is too wide.

- **D — Dependency Inversion Principle (DIP)**: High-level modules MUST NOT
  depend on low-level modules. Both MUST depend on abstractions.
  - This is enforced structurally by Principle VIII: controllers depend on
    `Application.Contracts` interfaces, application services depend on
    query/repository interfaces, command handlers depend on repository
    interfaces — never on concrete EF Core classes.
  - The `HttpApi` project MUST NOT reference `Application` or
    `EntityFrameworkCore`. The `Application` project MUST NOT reference
    `EntityFrameworkCore`.
  - Angular services MUST inject `HttpClient` (abstraction) — never
    instantiate HTTP connections directly.
  - All cross-layer wiring MUST go through ABP's dependency injection
    container. Manual `new ConcreteClass()` for services is forbidden.
  - **Test**: Every class with external dependencies MUST be unit-testable
    by injecting mock implementations of its interface dependencies.

**Rationale**: SOLID principles prevent the codebase from degrading into
tightly coupled, untestable, and rigid structures as the product grows.
They are especially critical in a financial application where correctness,
auditability, and maintainability are non-negotiable. Each principle
reinforces the others — SRP keeps classes focused, OCP prevents regression
from new features, LSP ensures substitutability, ISP keeps interfaces
lean, and DIP enforces the layered architecture mandated by ABP and
Principle VIII.

## Technology Stack & Constraints

| Layer | Technology | Version Policy |
|---|---|---|
| Backend runtime | .NET (LTS) | Track latest LTS; upgrade within 6 months |
| Application framework | ABP Framework | Match .NET LTS release cycle |
| ORM | Entity Framework Core | ABP repository abstractions; no raw SQL in Domain/Application |
| Database | SQL Server or PostgreSQL | Via ABP DB provider; all amounts in piasters (`bigint`) |
| Frontend framework | Angular (LTS) | Upgrade within 3 months of LTS release |
| UI components | ABP theme / Angular Material | Single design system; no mixing |
| Component catalog | Storybook | Required for all shared components |
| State management | Angular services + RxJS | NgRx only if demonstrably needed (ADR required) |
| Testing (backend) | xUnit + ABP test infrastructure | Mandatory |
| Testing (frontend) | Jest | Mandatory |
| E2E testing | Cypress or Playwright | Mandatory for P1 user stories |
| CI/CD | GitHub Actions | Required; no merges without green pipeline |
| Mock API | json-server, MSW, or Angular in-memory-web-api | Required for frontend-first development |

**Third-party packages**: New npm or NuGet dependencies MUST be evaluated for
license (MIT/Apache preferred), maintenance activity (commit within 6 months),
and security (no known CVEs). Evidence MUST appear in the PR description.

## Security & Audit Requirements

- **OTP rate limiting**: OTP request endpoints MUST enforce rate limiting:
  maximum 5 attempts per phone number per 15-minute window. Exceeded attempts
  MUST lock the phone number for 30 minutes and log a security event.
- **KYC image handling**: National ID images MUST be uploaded via signed,
  short-lived URLs (presigned S3/Azure Blob). Images MUST NOT be served via
  publicly accessible URLs. Storage MUST be encrypted at rest. Images MUST
  be deleted when no longer required by retention policy.
- **Audit logging**: All sensitive actions MUST produce audit log entries
  (ABP audit logging module). Sensitive actions include: login, OTP
  verification, KYC submission, deposit, withdrawal, share purchase, share
  sale, fee policy change, permission change, and admin impersonation.
- **Input validation**: All API inputs MUST be validated via FluentValidation
  or ABP validation attributes. Unvalidated inputs reaching domain logic are
  a blocking defect.
- **Secrets**: No secrets, API keys, connection strings, or credentials in
  source code or configuration files committed to Git. Use environment
  variables or a secrets manager.

## Quality Gates & Testing Requirements

Every feature MUST pass these gates in order. A failure blocks progression.

1. **Spec gate**: Feature spec (`spec.md`) approved before implementation.
2. **Plan gate**: Implementation plan (`plan.md`) with constitution check
   approved before coding.
3. **Contract gate**: API contracts (DTOs, endpoints, error codes) defined
   and agreed before any frontend or backend work begins.
4. **Frontend gate**: UI complete against mock API with Storybook stories,
   loading/empty/error states verified, RTL layout verified.
5. **Test gate**: Required tests written and confirmed FAILING before
   implementation:
   - **Unit tests** for fee calculations (all fee types, edge cases:
     zero values, maximum values, rounding).
   - **Unit tests** for projection engine (varying inputs, boundary
     conditions).
   - **Unit tests** for state machine transitions (valid + invalid
     transitions).
   - **Integration tests** for ledger operations (double-entry balance,
     idempotency, compensating entries).
   - **E2E tests** for the critical path: onboarding (OTP + KYC) →
     deposit → invest → portfolio view → exit request.
6. **Code review gate**: All PRs require one approving review. Reviewers
   MUST check constitution compliance (principles I–IX).
7. **CI gate**: Build + unit tests (≥ 80 % coverage on Domain +
   Application layers) + linting + Angular bundle budget (≤ 250 KB
   gzipped) + Lighthouse (score ≥ 80, LCP ≤ 2.5 s).
8. **Security gate**: No HIGH/CRITICAL dependency vulnerabilities.
   OTP rate limiting verified. KYC upload path verified.

**Definition of Done**:
- Code compiles with zero warnings in strict mode.
- All tests pass (unit, integration, E2E for P1 stories).
- Code reviewed and approved.
- All UI screens verified in RTL Arabic with EGP formatting.
- Loading, empty, and error states present on every async operation.
- No new linting violations introduced.
- Fee breakdowns shown before every confirmation step.

## Performance Budgets

These are hard constraints enforced in CI:

| Metric | Target | Enforcement |
|---|---|---|
| API response time (p95) | ≤ 200 ms (≤ 100 concurrent users) | Load test in staging |
| DB round-trips per request | ≤ 10 | MiniProfiler / EF Core logging in CI |
| Angular initial bundle (gzip) | ≤ 250 KB | Angular `--budget` flag in CI |
| Largest Contentful Paint | ≤ 2.5 s (mid-tier mobile) | Lighthouse CI ≥ 80 |
| N+1 queries | Zero tolerance | Blocking defect |
| Observable memory leaks | Zero tolerance | `takeUntilDestroyed` / `AsyncPipe` enforced |

## Governance

This constitution is the highest-authority document governing MshNawy
development. It supersedes all team conventions, README guidance, and verbal
agreements.

**Amendment procedure**:
1. Open a GitHub issue labelled `constitution-amendment` with the proposed
   change and rationale.
2. Allow a 48-hour comment period.
3. Approval from at least two team members (or the project owner on solo
   projects).
4. Update version per semver rules, set `LAST_AMENDED_DATE`, and update the
   Sync Impact Report at the top of this file.
5. Propagate impacts to templates via `/speckit.constitution`.

**Versioning policy**:
- MAJOR: Removal or redefinition of an existing principle.
- MINOR: Addition of a new principle or material expansion.
- PATCH: Clarifications, wording fixes, non-semantic refinements.

**Compliance review**: Verified at every PR review (gate 6) and in quarterly
retrospectives. Violations are logged as GitHub issues with the label
`constitution-violation` and MUST be resolved within the current sprint.

**Exceptions**: Any exception MUST be documented in the PR, approved by the
project owner, and tracked in the Complexity Tracking table of `plan.md`.

---

**Version**: 2.9.0 | **Ratified**: 2026-02-28 | **Last Amended**: 2026-03-26

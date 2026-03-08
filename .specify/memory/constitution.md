<!--
SYNC IMPACT REPORT
==================
Version change: 1.0.0 → 2.0.0 (MAJOR — all principles redefined for domain-specific governance)

Modified principles:
  - I. Code Quality → I. Arabic-Only & Egyptian Market (redefined)
  - II. Testing Standards → II. Financial Integrity — Double-Entry Ledger (redefined)
  - III. UX Consistency → III. Atomic & Idempotent Financial Operations (redefined)
  - IV. Performance Requirements → IV. Configurable Fee Policy (redefined)
  - V. ABP Framework Compliance → V. Dynamic Financial Projections (redefined)

Added sections:
  - VI. Frontend-First Delivery Strategy (new principle)
  - VII. Code Quality & ABP Compliance (consolidated from v1.0 principles I + V)
  - Security & Audit Requirements (new section)
  - Quality Gates (rewritten to include E2E onboarding flow, fee calc tests)

Removed sections:
  - None (v1.0 content redistributed into new structure)

Templates reviewed:
  - .specify/templates/plan-template.md       ✅ Compatible — Constitution Check
                                                  must gate against principles I–VII
  - .specify/templates/spec-template.md       ✅ Compatible — User stories must
                                                  include Arabic-locale acceptance
                                                  criteria and financial correctness
  - .specify/templates/tasks-template.md      ✅ Compatible — test tasks REQUIRED
                                                  per Principle VI and Quality Gates;
                                                  Phase 2 must include ledger setup
  - .specify/templates/agent-file-template.md ✅ Compatible — technology section
                                                  aligns with stack constraints

Deferred TODOs:
  - None. All placeholders resolved.
-->

# RealInvest Constitution

## Core Principles

### I. Arabic-Only & Egyptian Market (NON-NEGOTIABLE)

RealInvest is an Arabic-first product targeting Egyptian investors exclusively.
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

All monetary movements in RealInvest MUST be recorded via a double-entry ledger.
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

RealInvest fees MUST follow the business model and MUST be implemented as
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

RealInvest follows a frontend-first development methodology. The UI is built
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
- **SOLID**: God classes and God services are forbidden. Single-responsibility
  and dependency-inversion are mandatory.
- **Naming**: Names MUST be self-documenting. No abbreviations except
  universally accepted acronyms (`Id`, `Dto`, `Api`, `Otp`, `Kyc`, `Egp`).
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
   MUST check constitution compliance (principles I–VII).
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

This constitution is the highest-authority document governing RealInvest
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

**Version**: 2.0.0 | **Ratified**: 2026-02-28 | **Last Amended**: 2026-02-28

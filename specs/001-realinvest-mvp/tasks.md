# Tasks: MshNawy (مش ناوي) — Egyptian Fractional Real Estate Investment Platform

**Input**: Design documents from `/specs/001-realinvest-mvp/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/api-contracts.md, quickstart.md

**Tests**: Required by constitution (Quality Gates §5). Unit tests for fee calcs, projection engine, state machines. Integration tests for ledger. E2E tests for critical path.

**Organization**: Tasks grouped by user story. Each story follows frontend-first delivery: Contracts → Mocks → Angular UI → Backend Domain → Backend Application → API.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Exact file paths included in descriptions

---

## Phase 0: Codebase Rename (Blocking — must complete before Phase 3)

**Purpose**: Rename existing scaffold from "RealInvest" to "MshNawy" across all code files

- [X] T000 Rename ABP solution from RealInvest to MshNawy:
  1. ~~Rename `aspnet-core/RealInvest.sln` → `aspnet-core/MshNawy.sln`~~ ✅
  2. ~~Rename all `.csproj` files: `RealInvest.*.csproj` → `MshNawy.*.csproj`~~ ✅
  3. ~~Replace all `namespace RealInvest.` → `namespace MshNawy.` across all `.cs` files~~ ✅
  4. ~~Replace all `using RealInvest.` → `using MshNawy.` across all `.cs` files~~ ✅
  5. ~~Replace `[DependsOn(typeof(RealInvest` → `[DependsOn(typeof(MshNawy` in module classes~~ ✅
  6. ~~Replace `Database=RealInvest` → `Database=MshNawy` in appsettings.json~~ ✅
  7. ~~Replace `"name": "realinvest"` → `"name": "mshn-nawy"` in angular/package.json~~ ✅
  8. ~~Replace `RealInvest:` → `MshNawy:` error code prefix in ar.json and all source files~~ ✅ (source code uses MshNawy:NNNN format; api-contracts.md updated separately)
  9. ~~Run `dotnet build aspnet-core/` and `ng build` to verify zero errors after rename~~ ✅

---

## Phase 1: Setup (Project Initialization)

**Purpose**: Scaffold ABP solution and Angular app, configure tooling

- [X] T001 Scaffold ABP solution using `abp new MshNawy -t app -u angular --database-provider ef -dbms SqlServer --mobile none` in aspnet-core/
 - [X] T002 Verify ABP scaffold builds: `dotnet build` in aspnet-core/ and `npm install && ng build` in angular/
 - [X] T003 [P] Configure Angular for Arabic RTL: set `dir="rtl"` on root html element, add `ar-EG` locale registration, configure CSS logical properties lint rule in angular/.eslintrc.json
 - [X] T004 [P] Configure ABP localization: create `ar.json` resource file in aspnet-core/src/MshNawy.Domain.Shared/Localization/MshNawy/ar.json with all error codes from MshNawyErrorCodes.cs
 - [X] T005 [P] Install and configure MSW in angular/: `npm install msw --save-dev`, create angular/src/app/mock/browser.ts with service worker setup, add mock toggle to angular/src/environments/environment.ts
 - [X] T006 [P] Install and configure Storybook for Angular: `npx storybook@latest init`, configure for RTL Arabic preview in angular/.storybook/preview.ts
 - [X] T007 [P] Configure FluentValidation in aspnet-core/src/MshNawy.Application/ and register validators in module class
 - [X] T008 Create shared Angular pipes: EGP currency formatting pipe in angular/src/app/shared/pipes/egp.pipe.ts, Arabic date pipe in angular/src/app/shared/pipes/arabic-date.pipe.ts
 - [X] T009 Create shared Angular TypeScript interfaces matching common API patterns (pagination, error response) in angular/src/app/shared/models/api.models.ts

**Checkpoint**: Project scaffolded, Arabic RTL configured, MSW + Storybook ready, shared utilities in place

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that ALL user stories depend on — ledger, fee engine, auth guards, shared components

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

### Domain Foundation

- [X] T010 Create all shared enums in aspnet-core/src/MshNawy.Domain.Shared/: KycStatus, DepositMethod, DepositStatus, WithdrawalMethod, WithdrawalStatus, RiskLevel, OfferingStatus, OrderStatus, InstallmentStatus, ExitStatus, TicketCategory, TicketStatus, LedgerEntryType, NotificationEventType
- [X] T011 Create MshNawy error codes constants class in aspnet-core/src/MshNawy.Domain.Shared/MshNawyErrorCodes.cs matching all error codes from contracts/api-contracts.md
- [X] T012 Implement LedgerEntry entity (immutable, no public setters) in aspnet-core/src/MshNawy.Domain/Wallet/LedgerEntry.cs per data-model.md
- [X] T013 Implement LedgerService domain service in aspnet-core/src/MshNawy.Domain/Wallet/LedgerService.cs: PostEntry (creates debit+credit pair), PostCompensatingEntry (references original), ValidateBalance (sum check)
- [X] T014 Implement BalanceCalculator domain service in aspnet-core/src/MshNawy.Domain/Wallet/BalanceCalculator.cs: derives Available, Reserved, Invested, PendingWithdrawal from ledger entries for a user
- [X] T015 Implement FeePolicy aggregate root in aspnet-core/src/MshNawy.Domain/Fees/FeePolicy.cs per data-model.md with effective-date validation
- [X] T016 Implement FeeCalculator domain service in aspnet-core/src/MshNawy.Domain/Fees/FeeCalculator.cs: CalculateEntryFee, CalculatePaymentFee, CalculateExitFee (brokerage + platform split), CalculateWithdrawalFee
- [X] T017 Configure EF Core DbContext: register LedgerEntry and FeePolicy entity mappings in aspnet-core/src/MshNawy.EntityFrameworkCore/MshNawyDbContext.cs, add unique index on LedgerEntry.IdempotencyKey
- [X] T018 Create initial database migration in aspnet-core/src/MshNawy.EntityFrameworkCore/ (migrations exist: 20260228_InitialLedgerAndFees + 20260308203948_FeesPoliciesTable)

### Tests — Foundational

- [X] T019 [P] Unit tests for FeeCalculator: all fee types, zero values, max values, rounding behavior in aspnet-core/test/MshNawy.Domain.Tests/Fees/FeeCalculatorTests.cs
- [X] T020 [P] Unit tests for LedgerService: debit+credit balance, compensating entries, idempotency key duplicate rejection in aspnet-core/test/MshNawy.Domain.Tests/Wallet/LedgerServiceTests.cs
- [X] T021 [P] Unit tests for BalanceCalculator: derive balances from mixed ledger entries, empty ledger, single entry in aspnet-core/test/MshNawy.Domain.Tests/Wallet/BalanceCalculatorTests.cs

### Angular Foundation

- [X] T022 Create Angular auth guard (redirect to login if unauthenticated) in angular/src/app/shared/guards/auth.guard.ts
- [X] T023 Create Angular KYC guard (redirect to KYC flow if KYC not approved) in angular/src/app/shared/guards/kyc.guard.ts
- [X] T024 Create shared Angular components with Storybook stories: LoadingSpinner, EmptyState, ErrorState, ConfirmationDialog in angular/src/app/shared/components/ (each with default, RTL, mobile stories)
- [X] T025 Create app shell layout with RTL Arabic navigation bar (home, offerings, wallet, portfolio, support, notifications bell, profile) in angular/src/app/shared/components/app-shell/
- [X] T026 Create MSW base handler utilities and seed data structure in angular/src/app/mock/handlers/base.ts and angular/src/app/mock/data/seed.ts with deterministic Arabic mock data

**Checkpoint**: Foundation ready — ledger, fees, auth guards, shared components, MSW infrastructure. User story implementation can begin.

---

## Phase 3: User Story 1 — Onboarding & KYC Verification (Priority: P1) 🎯 MVP

**Goal**: Egyptian users register via phone OTP, verify identity, complete KYC document submission, and receive admin approval before accessing financial features.

**Independent Test**: A user can register, receive OTP, verify phone, submit KYC documents, and see KYC status update — all without any other features active.

### Contracts & Mocks — US1

- [X] T027 [P] [US1] Create auth DTOs (SendOtpInput, VerifyOtpInput, AuthResult) and KYC DTOs (KycStatusDto, KycSubmitInput) in aspnet-core/src/MshNawy.Application.Contracts/Identity/
- [X] T028 [P] [US1] Create TypeScript interfaces for auth and KYC matching DTOs in angular/src/app/shared/models/identity.models.ts
- [X] T029 [US1] Create MSW handlers for POST /auth/send-otp, POST /auth/verify-otp, GET /kyc/status, POST /kyc/upload, POST /kyc/submit, GET /kyc/image/{token} in angular/src/app/mock/handlers/identity.handlers.ts — mock OTP accepts any 6 digits, returns hardcoded mock JWT (`mock-jwt-token`), deterministic userId from seed data, and mock KYC status. Mock upload handler returns deterministic file tokens.

### Angular Frontend — US1

- [X] T030 [US1] Build OTP login screen: phone number input (auto-prepend +20, user enters 10 digits), OTP entry (6-digit), countdown timer (3 min), error states — all Arabic RTL in angular/src/app/onboarding/login/
- [X] T031 [US1] Build KYC submission flow: multi-step form (name, DOB, national ID 14-digit, front/back photo upload via POST /kyc/upload, selfie upload via POST /kyc/upload) then submit with file tokens via POST /kyc/submit — with validation in angular/src/app/onboarding/kyc/
- [X] T032 [US1] Build KYC status display component: shows current status (Draft/Submitted/Approved/Rejected with reason/NeedsResubmission), resubmission option in angular/src/app/onboarding/kyc-status/
- [X] T033 [P] [US1] Create Storybook stories for OTP login and KYC components: default, error, loading, OTP locked, KYC rejected states in angular/src/app/onboarding/*.stories.ts

### File Storage Infrastructure — US1 (prerequisite for KYC upload)

- [X] T168 [US1] Implement IFileStorageService abstraction in aspnet-core/src/MshNawy.Domain/Shared/IFileStorageService.cs with methods: `StoreFileAsync`, `GetFileStreamAsync`, `DeleteFileAsync`. Implement LocalFileStorageService in aspnet-core/src/MshNawy.EntityFrameworkCore/Infrastructure/FileStorage/LocalFileStorageService.cs — stores files outside wwwroot, served only via authenticated API endpoint (no public URL — constitution §Security MUST). Register in DI.
- [X] T168b [US1] Implement KYC image upload/retrieval API: `POST /api/app/kyc/upload` (stores file via IFileStorageService, returns opaque fileToken), `GET /api/app/kyc/image/{fileToken}` (validates JWT + file ownership, streams file) in aspnet-core/src/MshNawy.HttpApi/Identity/KycImageController.cs — ensures no KYC image is publicly accessible

### Backend Domain — US1

- [X] T034 [US1] Create AppUser entity as a separate aggregate root in aspnet-core/src/MshNawy.Domain/Identity/AppUser.cs — references ABP IdentityUser.Id via FK (composition, not inheritance). Fields from data-model.md: KycStatus, KycRejectionReason, FullNameArabic, DateOfBirth, NationalIdNumber, NationalIdFrontImagePath, NationalIdBackImagePath, SelfiePath, OtpCode (hashed), OtpExpiresAt, OtpAttemptCount, OtpWindowStart, OtpLockedUntil. All KYC and OTP fields on this entity.
- [X] T035 [US1] Implement OtpService domain service in aspnet-core/src/MshNawy.Domain/Identity/OtpService.cs: GenerateOtp (returns plain OTP, stores SHA256 hash + expiration on AppUser), VerifyOtp (compares hash, checks expiration within 3 min), CheckRateLimit (5 attempts per 15-min window tracked via OtpAttemptCount/OtpWindowStart, 30 min lockout via OtpLockedUntil). OTP code stored as SHA256 hash on AppUser entity — never stored in plaintext.
- [X] T036 [US1] Implement KYC state machine with guarded transitions in AppUser domain methods: `SubmitKyc` (Draft→Submitted), `MoveToUnderReview` (Submitted→UnderReview, called by admin), `ApproveKyc` (UnderReview→Approved), `RejectKyc` (UnderReview→Rejected with reason), `RequestResubmission` (UnderReview→NeedsResubmission), `Resubmit` (Rejected/NeedsResubmission→Submitted) in aspnet-core/src/MshNawy.Domain/Identity/AppUser.cs — all six transitions must be guarded (invalid transition throws BusinessException)

### Backend Application & API — US1

- [X] T037 [US1] Implement AuthAppService in aspnet-core/src/MshNawy.Application/Identity/AuthAppService.cs: SendOtp (finds or creates ABP IdentityUser by phone, creates AppUser if needed, calls OtpService.GenerateOtp, delegates SMS to IOtpSender interface — mock impl for MVP), VerifyOtp (validates via OtpService, then generates JWT using ABP's ITokenService/SignInManager, returns accessToken + userId + kycStatus)
- [X] T038 [US1] Implement KycAppService in aspnet-core/src/MshNawy.Application/Identity/KycAppService.cs: GetStatus, Upload (stores file via IFileStorageService, returns opaque token), Submit (accepts file tokens + form data, calls AppUser.SubmitKyc)
- [X] T039 [US1] Create API controllers for auth and KYC in aspnet-core/src/MshNawy.HttpApi/Identity/AuthController.cs and KycController.cs
- [X] T040 [US1] EF Core migration for AppUser extension fields: `dotnet ef migrations add AddKycFields` ✅ (migration 20260309200923_AddKYC exists)

### Tests — US1

- [X] T041 [P] [US1] Unit tests for OtpService: rate limiting, lockout, expiration, valid/invalid OTP in aspnet-core/test/MshNawy.Domain.Tests/Identity/OtpServiceTests.cs
- [X] T042 [P] [US1] Unit tests for KYC state machine: all valid transitions, invalid transition rejection in aspnet-core/test/MshNawy.Domain.Tests/Identity/KycStateMachineTests.cs

### Admin — US1

- [X] T043 [US1] Create admin KYC review DTOs in aspnet-core/src/MshNawy.Application.Contracts/Identity/Admin/
- [X] T044 [US1] Implement admin KycReviewAppService (list submitted/under-review, view documents, MoveToUnderReview, approve/reject with reason, request resubmission) in aspnet-core/src/MshNawy.Application/Identity/Admin/KycReviewAppService.cs
- [X] T045 [US1] Build admin KYC review screen: list pending KYC submissions, view documents, approve/reject with reason in angular/src/app/admin/kyc-review/

**Checkpoint**: User Story 1 complete. Users can register via OTP, submit KYC, admin can review. KYC guard blocks unapproved users from financial features.

---

## Phase 3b: Post-Implementation Remediation (Blocking — must complete before Phase 4)

**Purpose**: Fix constitution violations, rename remnants, and missing infrastructure identified in post-Phase 3 analysis (2026-03-13)

### Constitution §I — Localization Violations

- [X] T184 [P] Replace all hardcoded Arabic strings with ABP localization service calls across Angular components:
  1. `angular/src/app/onboarding/login/onboarding-login.component.ts` — move error messages to localization keys
  2. `angular/src/app/onboarding/kyc/onboarding-kyc.component.ts` — move error messages to localization keys
  3. `angular/src/app/onboarding/kyc-status/kyc-status.component.ts` — move error messages to localization keys
  4. `angular/src/app/shared/components/error-state/error-state.component.ts` — replace default `@Input()` Arabic string with localization key
  5. Add all new localization keys to `aspnet-core/src/MshNawy.Domain.Shared/Localization/MshNawy/ar.json`
  6. Use ABP localization pipe (`abpLocalization` or `LocalizationService`) — no hardcoded strings in `.ts` files

### Rename Completion

- [X] T185 [P] Complete RealInvest → MshNawy rename in Angular UI:
  1. `angular/src/index.html` line 5: change `<title>RealInvest</title>` → `<title>MshNawy</title>`
  2. `angular/src/app/shared/components/app-shell/app-shell.component.html`: replace `RealInvest` brand text with localized app name
  3. Rename `ri-` CSS class prefix → `msn-` across app-shell template and stylesheet
  4. Rename component selector `ri-app-shell` → `msn-app-shell` and update all references in templates and app.component.html

### Guard Wiring

- [X] T186 Wire Angular guards to actual services:
  1. `angular/src/app/shared/guards/auth.guard.ts` — inject AuthService, check JWT token in localStorage/sessionStorage, redirect to `/onboarding/login` if absent/expired
  2. `angular/src/app/shared/guards/kyc.guard.ts` — inject KycService or call GET /kyc/status, redirect to `/onboarding/kyc` if status ≠ Approved
  3. Verify guards are applied on wallet, offerings, portfolio, subscription routes in routing module

### Idempotency Infrastructure

- [X] T187 Implement idempotency key middleware for financial endpoints (FR-013, Constitution §III):
  1. Create `IdempotencyMiddleware` in `aspnet-core/src/MshNawy.HttpApi.Host/` — reads `X-Idempotency-Key` header, checks DB for existing key, returns cached response on duplicate
  2. Create `IdempotencyRecord` entity (Key UUID, ResponseBody, StatusCode, CreatedAt, ExpiresAt 24h) in `aspnet-core/src/MshNawy.Domain/Shared/`
  3. Register entity in DbContext, add migration
  4. Apply middleware to all `POST`/`PUT` endpoints under `/api/app/wallet/`, `/api/app/deposits/`, `/api/app/withdrawals/`, `/api/app/orders/`
  5. Unit test: duplicate key returns original response; expired key allows re-processing

### Contract Cleanup

- [X] T188 [P] Remove `refreshToken` field from MVP scope:
  1. Remove `refreshToken` from `AuthResultDto` in `aspnet-core/src/MshNawy.Application.Contracts/Identity/AuthDtos.cs`
  2. Remove from `VerifyOtpCommandHandler` response construction
  3. Remove from `AuthResult` TypeScript interface in `angular/src/app/shared/models/identity.models.ts`
  4. Remove from MSW identity handler response in `angular/src/app/mock/handlers/identity.handlers.ts`
  5. Update `specs/001-realinvest-mvp/contracts/api-contracts.md` verify-otp response — remove `refreshToken` field, add note: "Refresh tokens deferred to post-MVP"

**Checkpoint**: All constitution violations resolved, rename complete, guards functional, idempotency infrastructure ready for financial endpoints.

---

## Phase 4: User Story 2 — Wallet Deposit (Priority: P1)

**Goal**: KYC-approved users deposit funds via InstaPay, Vodafone Cash, or Bank Transfer with proof upload and back-office settlement.

**Independent Test**: A KYC-approved user can initiate a deposit, see payment instructions with unique reference, upload proof, and observe deposit lifecycle — without investment features.

### Contracts & Mocks — US2

- [ ] T046 [P] [US2] Create Deposit DTOs (PaymentDetailsDto, CreateDepositInput, DepositDto), Wallet DTOs (WalletBalanceDto, TransactionDto) in aspnet-core/src/MshNawy.Application.Contracts/Wallet/
- [ ] T047 [P] [US2] Create TypeScript interfaces for wallet and deposits in angular/src/app/shared/models/wallet.models.ts
- [ ] T048 [US2] Create MSW handlers for wallet balance, deposit payment details, create deposit, list deposits in angular/src/app/mock/handlers/wallet.handlers.ts with lifecycle simulation (Created→PendingReview→Approved→Posted)

### Angular Frontend — US2

- [ ] T049 [US2] Build wallet dashboard: Available/Reserved/Invested balances display, transaction history list in angular/src/app/wallet/dashboard/
- [ ] T050 [US2] Build deposit flow: method selection (InstaPay/VodafoneCash/BankTransfer), payment instructions with reference code, proof image upload, confirmation in angular/src/app/wallet/deposit/
- [ ] T051 [US2] Build deposit status tracking component with lifecycle progress indicator in angular/src/app/wallet/deposit-status/
- [ ] T052 [P] [US2] Create Storybook stories for wallet and deposit components: balances (zero/loaded), deposit methods, upload states in angular/src/app/wallet/*.stories.ts

### Backend Domain — US2

- [ ] T053 [US2] Implement Deposit aggregate root with state machine (Created→PendingReview→Approved→Rejected→Posted) in aspnet-core/src/MshNawy.Domain/Deposits/Deposit.cs
- [ ] T054 [US2] Implement DepositManager domain service: CreateDeposit (generates reference code), SubmitProof, ApproveDeposit, RejectDeposit, PostDeposit (creates ledger entries via LedgerService) in aspnet-core/src/MshNawy.Domain/Deposits/DepositManager.cs

### Backend Application & API — US2

- [ ] T055 [US2] Implement WalletAppService: GetBalance (via BalanceCalculator), GetTransactions in aspnet-core/src/MshNawy.Application/Wallet/WalletAppService.cs
- [ ] T056 [US2] Implement DepositAppService: GetPaymentDetails, Create, ListUserDeposits in aspnet-core/src/MshNawy.Application/Deposits/DepositAppService.cs
- [ ] T057 [US2] Create API controllers for wallet and deposits in aspnet-core/src/MshNawy.HttpApi/Wallet/WalletController.cs and Deposits/DepositController.cs
- [ ] T058 [US2] EF Core: register Deposit entity mapping, add migration `dotnet ef migrations add AddDeposits`

### Tests — US2

- [ ] T059 [P] [US2] Unit tests for Deposit state machine: all transitions, invalid transitions in aspnet-core/test/MshNawy.Domain.Tests/Deposits/DepositStateMachineTests.cs
- [ ] T060 [P] [US2] Integration tests for DepositManager: deposit posting creates correct ledger entries, idempotency in aspnet-core/test/MshNawy.Application.Tests/Deposits/DepositManagerIntegrationTests.cs

### Admin — US2

- [ ] T061 [US2] Implement admin DepositReviewAppService (list pending, approve/reject with notes) in aspnet-core/src/MshNawy.Application/Deposits/Admin/DepositReviewAppService.cs
- [ ] T062 [US2] Build admin deposit review screen: list pending deposits, view proof images, approve/reject in angular/src/app/admin/deposit-review/

**Checkpoint**: User Story 2 complete. Users can deposit funds, see wallet balances. Admin can approve/reject deposits. Ledger entries created correctly.

---

## Phase 5: User Story 3 — Browse Offerings with Dynamic Projections (Priority: P1)

**Goal**: Investors browse real estate offerings with dynamically computed projections across conservative/base/optimistic scenarios, fee breakdowns, and payment timelines.

**Independent Test**: A user can browse offerings, view dynamic projections under different scenarios, see fee breakdowns and payment timelines — without actually investing.

### Contracts & Mocks — US3

- [ ] T063 [P] [US3] Create Offering DTOs (OfferingListDto, OfferingDetailDto, ProjectionResultDto, PaymentTimelineDto, FeeBreakdownDto) in aspnet-core/src/MshNawy.Application.Contracts/Offerings/
- [ ] T064 [P] [US3] Create TypeScript interfaces for offerings and projections in angular/src/app/shared/models/offering.models.ts
- [ ] T065 [US3] Create MSW handlers for offering list, offering detail, projections (3 scenarios) in angular/src/app/mock/handlers/offering.handlers.ts with 3 deterministic seed offerings

### Angular Frontend — US3

- [ ] T066 [US3] Build offerings list page: property cards with name, location, share price, available shares, risk level, projected return range in angular/src/app/offerings/list/
- [ ] T067 [US3] Build offering detail page: property info, scenario toggle (Conservative/Base/Optimistic), projection charts, distribution schedule, exit value range in angular/src/app/offerings/detail/
- [ ] T068 [US3] Build fee breakdown expandable section component and payment timeline chart component in angular/src/app/offerings/components/
- [ ] T069 [US3] Add disclaimer component displayed on every offering page in angular/src/app/offerings/components/disclaimer/
- [ ] T070 [P] [US3] Create Storybook stories for offering components: card, detail, projections, fee breakdown, timeline chart, disclaimer in angular/src/app/offerings/*.stories.ts

### Backend Domain — US3

- [ ] T071 [US3] Implement Offering aggregate root with OfferingFinancialModel owned entity, OfferingImage collection, state machine (Draft→Open→Closed→Settled) in aspnet-core/src/MshNawy.Domain/Offerings/Offering.cs and OfferingFinancialModel.cs
- [ ] T072 [US3] Implement ProjectionEngine pure domain service: takes ProjectionInput (model inputs + scenario params + fee policy), returns ProjectionResult (annual return range, distribution schedule, exit values, fee impact, payment timeline) in aspnet-core/src/MshNawy.Domain/Offerings/ProjectionEngine.cs

### Backend Application & API — US3

- [ ] T073 [US3] Implement OfferingAppService: List (paginated, filtered by status), GetDetail, GetProjections (with scenario param) in aspnet-core/src/MshNawy.Application/Offerings/OfferingAppService.cs
- [ ] T074 [US3] Create OfferingController in aspnet-core/src/MshNawy.HttpApi/Offerings/OfferingController.cs
- [ ] T075 [US3] EF Core: register Offering, OfferingFinancialModel, OfferingImage entity mappings, add migration `dotnet ef migrations add AddOfferings`

### Tests — US3

- [ ] T076 [P] [US3] Unit tests for ProjectionEngine: varying inputs, boundary conditions, all 3 scenarios, deterministic output verification in aspnet-core/test/MshNawy.Domain.Tests/Offerings/ProjectionEngineTests.cs
- [ ] T077 [P] [US3] Unit tests for Offering state machine: valid/invalid transitions in aspnet-core/test/MshNawy.Domain.Tests/Offerings/OfferingStateMachineTests.cs

### Admin — US3

- [ ] T078 [US3] Implement admin OfferingManagementAppService: Create, Update, ChangeStatus (Open/Close) in aspnet-core/src/MshNawy.Application/Offerings/Admin/OfferingManagementAppService.cs
- [ ] T079 [US3] Build admin offering management screen: create/edit offering with all financial model inputs, open/close offerings in angular/src/app/admin/offering-management/

**Checkpoint**: User Story 3 complete. Users can browse offerings with dynamic projections. Admin can create/manage offerings.

---

## Phase 6: User Story 4 — Subscribe to Investment (Priority: P1)

**Goal**: Investors subscribe to offerings with knowledge check, fund reservation, settlement, and share issuance — with transparent fee application and payment plan generation.

**Independent Test**: A user with available balance can select an offering, complete knowledge check, create order, see funds reserved, and observe settlement with shares issued — verifying complete ledger trail.

### Contracts & Mocks — US4

- [ ] T080 [P] [US4] Create Order DTOs (CreateOrderInput, OrderDto, OrderSummaryDto, PaymentScheduleDto), KnowledgeCheck DTOs (KnowledgeCheckStatusDto, SubmitKnowledgeCheckInput) in aspnet-core/src/MshNawy.Application.Contracts/Orders/
- [ ] T081 [P] [US4] Create TypeScript interfaces for orders and knowledge check in angular/src/app/shared/models/order.models.ts
- [ ] T082 [US4] Create MSW handlers for knowledge check status/submit, create order, list orders in angular/src/app/mock/handlers/order.handlers.ts with fund reservation simulation

### Angular Frontend — US4

- [ ] T083 [US4] Build knowledge check questionnaire component (risk awareness questions in Arabic, pass/fail logic) in angular/src/app/subscription/knowledge-check/
- [ ] T084 [US4] Build subscription flow: share count selection, order summary (share price, entry fee, total cost, payment plan schedule), confirmation in angular/src/app/subscription/order-flow/
- [ ] T085 [US4] Build order status tracking component with lifecycle indicator (Created→Reserved→Settled/Failed) in angular/src/app/subscription/order-status/
- [ ] T086 [P] [US4] Create Storybook stories for subscription components: knowledge check, order summary, insufficient funds, payment plan breakdown in angular/src/app/subscription/*.stories.ts

### Backend Domain — US4

- [ ] T087 [US4] Implement InvestmentOrder aggregate root with Installment owned entities, state machine (Created→Reserved→Submitted→Settled/Failed) in aspnet-core/src/MshNawy.Domain/Orders/InvestmentOrder.cs and Installment.cs
- [ ] T088 [US4] Implement OrderManager domain service: CreateOrder (validates balance, calculates fees, generates installments), ReserveFunds (atomic ledger entries Available→Reserved with idempotency), SettleOrder (Reserved→Invested, issue shares), FailOrder (compensating entries) in aspnet-core/src/MshNawy.Domain/Orders/OrderManager.cs
- [ ] T089 [US4] Implement KnowledgeCheckRecord entity and KnowledgeCheckService domain service (check if risk level already acknowledged, record completion) in aspnet-core/src/MshNawy.Domain/Orders/KnowledgeCheckRecord.cs and KnowledgeCheckService.cs
- [ ] T090 [US4] Implement Holding aggregate root: created on order settlement, links to offering and order in aspnet-core/src/MshNawy.Domain/Portfolio/Holding.cs

### Backend Application & API — US4

- [ ] T091 [US4] Implement KnowledgeCheckAppService: GetStatus, Submit in aspnet-core/src/MshNawy.Application/Orders/KnowledgeCheckAppService.cs
- [ ] T092 [US4] Implement OrderAppService: Create (validates KYC, knowledge check, balance; calls OrderManager), List in aspnet-core/src/MshNawy.Application/Orders/OrderAppService.cs
- [ ] T093 [US4] Create OrderController and KnowledgeCheckController in aspnet-core/src/MshNawy.HttpApi/Orders/
- [ ] T094 [US4] EF Core: register InvestmentOrder, Installment, KnowledgeCheckRecord, Holding entity mappings, add migration `dotnet ef migrations add AddOrdersAndHoldings`

### Tests — US4

- [ ] T095 [P] [US4] Unit tests for OrderManager: fund reservation, settlement, failure with compensating entries, idempotency key duplicate, insufficient balance, **offering without payment plan = full upfront charge at subscription (FR-030)** in aspnet-core/test/MshNawy.Domain.Tests/Orders/OrderManagerTests.cs
- [ ] T096 [P] [US4] Unit tests for InvestmentOrder state machine and Installment generation in aspnet-core/test/MshNawy.Domain.Tests/Orders/OrderStateMachineTests.cs
- [ ] T097 [P] [US4] Integration tests for order flow: create→reserve→settle with ledger verification, create→reserve→fail with compensating entries in aspnet-core/test/MshNawy.Application.Tests/Orders/OrderFlowIntegrationTests.cs
- [ ] T179 [P] [US4] Unit tests for KnowledgeCheckService: risk-level escalation (skip for same/lower risk, re-present for higher risk), first-time check required in aspnet-core/test/MshNawy.Domain.Tests/Orders/KnowledgeCheckServiceTests.cs

### Admin — US4

- [ ] T098 [US4] Implement admin OrderSettlementAppService: ListReserved, Settle, Fail in aspnet-core/src/MshNawy.Application/Orders/Admin/OrderSettlementAppService.cs
- [ ] T099 [US4] Build admin order settlement screen: list reserved orders, settle/fail with reason in angular/src/app/admin/order-settlement/

**Checkpoint**: User Story 4 complete. Full investment flow works: knowledge check → order → reserve → settle → shares issued. Ledger trail verified.

---

## Phase 7: User Story 5 — Portfolio View & Statements (Priority: P2)

**Goal**: Investors view holdings, track performance with dynamic projections, see fee history, and export Arabic HTML statements.

**Independent Test**: A user with issued shares can view portfolio dashboard, expand holding details, see activity history, and export a statement.

### Contracts & Mocks — US5

- [ ] T100 [P] [US5] Create Portfolio DTOs (PortfolioSummaryDto, HoldingDto, HoldingActivityDto) and Statement DTOs in aspnet-core/src/MshNawy.Application.Contracts/Portfolio/
- [ ] T101 [P] [US5] Create TypeScript interfaces for portfolio in angular/src/app/shared/models/portfolio.models.ts
- [ ] T102 [US5] Create MSW handlers for portfolio summary, holdings list, holding activity, statement export in angular/src/app/mock/handlers/portfolio.handlers.ts

### Angular Frontend — US5

- [ ] T103 [US5] Build portfolio summary page: total invested, projected value range, total fees, active holdings count in angular/src/app/portfolio/summary/
- [ ] T104 [US5] Build holding detail expandable view: shares owned, cost basis, paid-to-date, fees, projected value, activity timeline in angular/src/app/portfolio/holding-detail/
- [ ] T105 [US5] Build statement export: date range picker, generate Arabic HTML statement, download/print in angular/src/app/portfolio/statement/
- [ ] T106 [P] [US5] Create Storybook stories for portfolio components: empty portfolio, single holding, multiple holdings, statement preview in angular/src/app/portfolio/*.stories.ts

### Backend — US5

- [ ] T107 [US5] Implement PortfolioAppService: GetSummary (aggregates holdings with projections), ListHoldings, GetHoldingActivity in aspnet-core/src/MshNawy.Application/Portfolio/PortfolioAppService.cs
- [ ] T108 [US5] Implement StatementGenerator domain service: generates Arabic HTML statement with holdings, transactions, fee breakdown in aspnet-core/src/MshNawy.Domain/Portfolio/StatementGenerator.cs
- [ ] T109 [US5] Create PortfolioController in aspnet-core/src/MshNawy.HttpApi/Portfolio/PortfolioController.cs

### Tests — US5

- [ ] T176 [P] [US5] Unit tests for StatementGenerator: Arabic HTML output correctness, holdings summary, fee breakdown, empty portfolio in aspnet-core/test/MshNawy.Domain.Tests/Portfolio/StatementGeneratorTests.cs
- [ ] T177 [P] [US5] Unit tests for PortfolioAppService: aggregation with projections, multiple holdings, zero holdings in aspnet-core/test/MshNawy.Application.Tests/Portfolio/PortfolioAppServiceTests.cs

**Checkpoint**: User Story 5 complete. Investors can view portfolio, track holdings, export Arabic statements.

---

## Phase 8: User Story 6 — Property Sale & Proceeds Distribution (Priority: P2)

**Goal**: Closed-end exit model. Investors vote to sell the property. When majority (>50%) votes to sell — or maturity is reached — the admin initiates property sale. Upon actual sale, proceeds are distributed pro-rata to all holders minus 5% exit fee. No individual early exit.

**Independent Test**: A user with holdings can view sale vote status, cast a vote, see the property sale lifecycle, and upon sale completion see net proceeds credited to Available balance.

### Contracts & Mocks — US6

- [ ] T110 [P] [US6] Create PropertySale DTOs (SaleVoteStatusDto, CastVoteInput, PropertySaleDto, SaleDistributionDto) in aspnet-core/src/MshNawy.Application.Contracts/PropertySales/
- [ ] T111 [P] [US6] Create TypeScript interfaces for sale votes and property sales in angular/src/app/shared/models/property-sale.models.ts
- [ ] T112 [US6] Create MSW handlers for sale vote status, cast vote, property sale status, distribution preview in angular/src/app/mock/handlers/property-sale.handlers.ts

### Angular Frontend — US6

- [ ] T113 [US6] Build sale vote component on holding detail: current vote percentage, threshold indicator, cast/withdraw vote button, maturity date display in angular/src/app/property-sales/vote/
- [ ] T114 [US6] Build property sale status tracking with lifecycle indicator (Initiated→Listed→Sold→Distributing→Settled) and distribution preview (pro-rata calculation, exit fee breakdown) in angular/src/app/property-sales/status/
- [ ] T115 [P] [US6] Create Storybook stories for property sale components: vote progress, threshold reached, sale in progress, distribution preview, settled state in angular/src/app/property-sales/*.stories.ts

### Backend — US6

- [ ] T116 [US6] Implement SaleVote entity and PropertySale aggregate root with SaleDistribution owned entities, state machine (Initiated→Listed→Sold→Distributing→Settled/Cancelled) in aspnet-core/src/MshNawy.Domain/PropertySales/
- [ ] T117 [US6] Implement SaleManager domain service: CastVote, WithdrawVote, CheckVoteThreshold, InitiateSale, RecordSalePrice, CalculateDistributions (pro-rata with FeeCalculator), SettleSale (creates ledger entries for each holder, deactivates holdings, cancels remaining installments) in aspnet-core/src/MshNawy.Domain/PropertySales/SaleManager.cs
- [ ] T118a [US6] Implement SaleVoteAppService: CastVote, WithdrawVote, GetVoteStatus in aspnet-core/src/MshNawy.Application/PropertySales/SaleVoteAppService.cs
- [ ] T118b [US6] Implement PropertySaleAppService: GetSaleStatus, GetDistributionPreview in aspnet-core/src/MshNawy.Application/PropertySales/PropertySaleAppService.cs
- [ ] T118c [US6] Create SaleVoteController and PropertySaleController in aspnet-core/src/MshNawy.HttpApi/PropertySales/
- [ ] T119 [US6] EF Core: register SaleVote, PropertySale, SaleDistribution entity mappings, unique constraint on (UserId, OfferingId) for SaleVote, add migration `dotnet ef migrations add AddPropertySales`

### Tests — US6

- [ ] T120 [P] [US6] Unit tests for SaleManager: vote threshold calculation, pro-rata distribution, exit fee deduction, ledger entries, holding deactivation, installment cancellation in aspnet-core/test/MshNawy.Domain.Tests/PropertySales/SaleManagerTests.cs
- [ ] T182 [P] [US6] Unit tests for PropertySale state machine: valid/invalid transitions, cancellation, vote withdrawal after sale initiated in aspnet-core/test/MshNawy.Domain.Tests/PropertySales/PropertySaleStateMachineTests.cs

### Admin — US6

- [ ] T121 [US6] Implement admin PropertySaleAppService: ListOfferingsWithMajorityVote, InitiateSale, UpdateStatus (Listed/Sold with actual price), SettleDistribution in aspnet-core/src/MshNawy.Application/PropertySales/Admin/PropertySaleAdminAppService.cs
- [ ] T122 [US6] Build admin property sale management screen: list offerings ready for sale (majority vote or maturity), initiate sale, record actual sale price, preview distribution, settle in angular/src/app/admin/property-sale-management/

**Checkpoint**: User Story 6 complete. Closed-end exit model works: vote → sale → distribute actual proceeds. Zero pricing risk for platform.

---

## Phase 9: User Story 6b — Wallet Withdrawal (Priority: P2)

**Goal**: Users withdraw Available balance to bank account (no fee) or Vodafone Cash (5 EGP fee) with back-office approval.

**Independent Test**: A user with Available balance can request withdrawal, see applicable fees, track status, and upon approval see balance decrease.

### Contracts & Mocks — US6b

- [ ] T123 [P] [US6b] Create Withdrawal DTOs (CreateWithdrawalInput, WithdrawalDto) in aspnet-core/src/MshNawy.Application.Contracts/Withdrawals/
- [ ] T124 [P] [US6b] Create TypeScript interfaces for withdrawals in angular/src/app/shared/models/withdrawal.models.ts
- [ ] T125 [US6b] Create MSW handlers for create withdrawal, list withdrawals in angular/src/app/mock/handlers/withdrawal.handlers.ts

### Angular Frontend — US6b

- [ ] T126 [US6b] Build withdrawal flow: method selection (Bank Transfer/Vodafone Cash), amount input, destination details, fee display (0 or 5 EGP), net amount, confirmation in angular/src/app/wallet/withdrawal/
- [ ] T127 [US6b] Build withdrawal status tracking in wallet transaction history in angular/src/app/wallet/withdrawal-status/
- [ ] T128 [P] [US6b] Create Storybook stories for withdrawal components: method selection, fee display, insufficient balance in angular/src/app/wallet/withdrawal/*.stories.ts

### Backend — US6b

- [ ] T129 [US6b] Implement Withdrawal aggregate root with state machine (Created→PendingReview→Processing→Completed/Rejected) in aspnet-core/src/MshNawy.Domain/Withdrawals/Withdrawal.cs
- [ ] T130 [US6b] Implement WithdrawalManager domain service: CreateWithdrawal (validates balance, reserves funds via ledger), ApproveWithdrawal, RejectWithdrawal (compensating entries) in aspnet-core/src/MshNawy.Domain/Withdrawals/WithdrawalManager.cs
- [ ] T131 [US6b] Implement WithdrawalAppService and WithdrawalController in aspnet-core/src/MshNawy.Application/Withdrawals/ and aspnet-core/src/MshNawy.HttpApi/Withdrawals/
- [ ] T132 [US6b] EF Core: register Withdrawal entity mapping, add migration `dotnet ef migrations add AddWithdrawals`

### Tests — US6b

- [ ] T133 [P] [US6b] Unit tests for WithdrawalManager: bank transfer (0 fee), Vodafone Cash (500 piasters fee), insufficient balance, compensating entries on rejection in aspnet-core/test/MshNawy.Domain.Tests/Withdrawals/WithdrawalManagerTests.cs
- [ ] T183 [P] [US6b] Integration tests for WithdrawalManager: withdrawal reservation creates correct ledger entries (Available→PendingWithdrawal), approval settles correctly, rejection returns funds via compensating entries — mirrors DepositManager integration test coverage in aspnet-core/test/MshNawy.Application.Tests/Withdrawals/WithdrawalManagerIntegrationTests.cs

### Admin — US6b

- [ ] T134 [US6b] Implement admin WithdrawalReviewAppService: ListPending, Approve, Reject in aspnet-core/src/MshNawy.Application/Withdrawals/Admin/WithdrawalReviewAppService.cs
- [ ] T135 [US6b] Build admin withdrawal review screen: list pending, approve/reject in angular/src/app/admin/withdrawal-review/

**Checkpoint**: User Story 6b complete. Users can withdraw funds. Admin can approve/reject withdrawals.

---

## Phase 10: User Story 7 — Support & Complaints (Priority: P3)

**Goal**: Users submit support tickets with attachments, track status with SLA indicators, and communicate via threaded messages.

**Independent Test**: A user can create a ticket with text and attachments, view ticket status, and see updates from support staff.

### Contracts & Mocks — US7

- [ ] T136 [P] [US7] Create Support DTOs (CreateTicketInput, TicketDto, TicketDetailDto, TicketMessageDto, CreateMessageInput) in aspnet-core/src/MshNawy.Application.Contracts/Support/
- [ ] T137 [P] [US7] Create TypeScript interfaces for support in angular/src/app/shared/models/support.models.ts
- [ ] T138 [US7] Create MSW handlers for create ticket, list tickets, get ticket detail, post message in angular/src/app/mock/handlers/support.handlers.ts

### Angular Frontend — US7

- [ ] T139 [US7] Build ticket creation form: subject, category dropdown, description, file attachments (up to 3) in angular/src/app/support/create-ticket/
- [ ] T140 [US7] Build ticket list view: reference number, subject, status, SLA indicator, creation date in angular/src/app/support/ticket-list/
- [ ] T141 [US7] Build ticket detail view: conversation thread, reply with attachments, status updates in angular/src/app/support/ticket-detail/
- [ ] T142 [P] [US7] Create Storybook stories for support components: empty tickets, SLA breach indicator, conversation thread in angular/src/app/support/*.stories.ts

### Backend — US7

- [ ] T143 [US7] Implement SupportTicket aggregate root with TicketMessage and TicketAttachment owned entities, state machine (Open→InProgress→Resolved→Closed) in aspnet-core/src/MshNawy.Domain/Support/SupportTicket.cs
- [ ] T144 [US7] Implement SupportAppService: CreateTicket, ListTickets, GetDetail, PostMessage in aspnet-core/src/MshNawy.Application/Support/SupportAppService.cs
- [ ] T145 [US7] Create SupportController in aspnet-core/src/MshNawy.HttpApi/Support/SupportController.cs
- [ ] T146 [US7] EF Core: register SupportTicket, TicketMessage, TicketAttachment entity mappings, add migration `dotnet ef migrations add AddSupportTickets`

### Tests — US7

- [ ] T178 [P] [US7] Unit tests for SupportTicket state machine: valid/invalid transitions (Open→InProgress→Resolved→Closed), SLA deadline calculation in aspnet-core/test/MshNawy.Domain.Tests/Support/SupportTicketTests.cs

### Admin — US7

- [ ] T147 [US7] Implement admin SupportManagementAppService: ListTickets (filtered by status), Respond, UpdateStatus in aspnet-core/src/MshNawy.Application/Support/Admin/SupportManagementAppService.cs
- [ ] T148 [US7] Build admin support management screen: ticket list, respond to tickets, update status in angular/src/app/admin/support-management/

**Checkpoint**: User Story 7 complete. Users can submit and track support tickets. Admin can respond and manage tickets.

---

## Phase 11: Notifications & Installments (Cross-Cutting)

**Purpose**: In-app notification center and installment background job — both span multiple user stories

### Notifications

- [ ] T149 [P] Create Notification entity and NotificationService domain service (create notification on status transitions) in aspnet-core/src/MshNawy.Domain/Notifications/Notification.cs and NotificationService.cs
- [ ] T150 [P] Create Notification DTOs and NotificationAppService: List (paginated, unread filter, unread count), MarkRead, MarkAllRead in aspnet-core/src/MshNawy.Application/Notifications/ and aspnet-core/src/MshNawy.Application.Contracts/Notifications/
- [ ] T151 Create NotificationController in aspnet-core/src/MshNawy.HttpApi/Notifications/NotificationController.cs
- [ ] T152a Wire NotificationService into DepositManager: emit notification on deposit status change (Pending/Approved/Rejected/Posted) in aspnet-core/src/MshNawy.Domain/Deposits/DepositManager.cs
- [ ] T152b Wire NotificationService into WithdrawalManager: emit notification on withdrawal status change in aspnet-core/src/MshNawy.Domain/Withdrawals/WithdrawalManager.cs
- [ ] T152c Wire NotificationService into OrderManager: emit notification on order settled/failed, installment due/overdue in aspnet-core/src/MshNawy.Domain/Orders/OrderManager.cs
- [ ] T152d Wire NotificationService into SaleManager: emit notification on vote threshold reached, sale status change, proceeds distributed in aspnet-core/src/MshNawy.Domain/PropertySales/SaleManager.cs
- [ ] T152e Wire NotificationService into SupportTicket domain: emit notification when support agent responds in aspnet-core/src/MshNawy.Domain/Support/SupportTicket.cs
- [ ] T152f Wire NotificationService into KYC transitions: emit notification on KYC status change (Approved/Rejected/NeedsResubmission) in aspnet-core/src/MshNawy.Domain/Identity/AppUser.cs
- [ ] T153 Create MSW handlers for notifications in angular/src/app/mock/handlers/notification.handlers.ts
- [ ] T154 Build notification center UI: bell icon with unread badge in nav bar, dropdown/page with chronological notification list, mark read, tap to navigate in angular/src/app/notifications/
- [ ] T155 [P] Create Storybook stories for notification components: empty, unread badge counts, notification list in angular/src/app/notifications/*.stories.ts

### Installments

- [ ] T156 Implement InstallmentProcessor background job: daily check for due installments, attempt auto-deduction from Available balance via LedgerService, transition to Overdue/GracePeriod/Flagged on failure in aspnet-core/src/MshNawy.Domain/Orders/InstallmentProcessor.cs
- [ ] T157 Register InstallmentProcessor as recurring ABP background job in aspnet-core/src/MshNawy.Application/MshNawyApplicationModule.cs
- [ ] T158 EF Core: register Notification entity mapping, add migration `dotnet ef migrations add AddNotifications`

### Tests — Cross-Cutting

- [ ] T159 [P] Unit tests for InstallmentProcessor: due date processing, insufficient balance → grace period, grace period expiry → flagged in aspnet-core/test/MshNawy.Domain.Tests/Orders/InstallmentProcessorTests.cs

**Checkpoint**: Notification center live across all flows. Installment auto-processing scheduled.

---

## Phase 12: Admin Infrastructure & Fee Policy Management

**Purpose**: Admin authentication, audit logging, and fee policy management

- [ ] T160 Configure ABP admin permissions in aspnet-core/src/MshNawy.Application.Contracts/Permissions/MshNawyPermissions.cs: define permission groups for KYC, Deposits, Withdrawals, Orders, Exits, Support, Offerings, FeePolicy
- [ ] T161 Implement admin authentication: separate admin login, permission enforcement via ABP authorize attributes on all admin app services
- [ ] T162 Configure ABP audit logging (`AbpAuditingOptions`) for all sensitive **admin** actions (KYC approve/reject, deposit approve/reject, withdrawal approve/reject, order settle/fail, property sale settle, fee policy change) in aspnet-core/src/MshNawy.HttpApi.Host/
- [ ] T162b Configure ABP audit logging for all sensitive **user-initiated** actions: login, OTP send/verify, OTP rate-limit exceeded, KYC submission, KYC image upload/access, deposit created, withdrawal created, order created/reserved, sale vote cast/withdrawn — configure via `AbpAuditingOptions.IsEnabledForAnonymousUsers` and selective controller audit attributes in aspnet-core/src/MshNawy.HttpApi.Host/
- [ ] T163 Implement admin FeePolicyAppService: List (with version history), Create new policy with effective date in aspnet-core/src/MshNawy.Application/Fees/Admin/FeePolicyAppService.cs
- [ ] T164 Build admin fee policy management screen: view history, create new policy in angular/src/app/admin/fee-policy/
- [ ] T165 Build admin dashboard: summary counts (pending KYC, pending deposits, pending withdrawals, pending orders, open tickets) in angular/src/app/admin/dashboard/
- [ ] T166 Configure Angular lazy-loaded admin module with admin route guard in angular/src/app/admin/admin.module.ts and admin-routing.module.ts
- [ ] T181 [P] Configure English locale for admin panel: add `en.json` ABP localization resource, configure admin Angular module to support both Arabic and English per FR-036 in angular/src/app/admin/ and aspnet-core/src/MshNawy.Domain.Shared/Localization/MshNawy/en.json

**Checkpoint**: Admin panel fully functional with all review screens, fee policy management, and audit logging.

---

## Phase 13: Polish & Cross-Cutting Concerns

**Purpose**: E2E tests, performance, security hardening, final validation

- [ ] T167 E2E test: complete critical path — register (OTP) → KYC submit → admin approve KYC → deposit → admin approve deposit → browse offerings → subscribe → admin settle order → view portfolio → vote to sell → admin initiate property sale → admin settle distribution in angular/e2e/critical-path.spec.ts
- [ ] T169 [P] Add FluentValidation validators for all DTOs (phone format, national ID 14 digits, file size 5MB, required fields) in aspnet-core/src/MshNawy.Application/Validators/
- [ ] T170 [P] Configure Angular bundle budgets (≤250KB gzip) in angular/angular.json and verify with `ng build --configuration=production`
- [ ] T171 [P] Add Angular loading, empty, and error states to all async operations across all feature modules (verify each page has all 3 states)
- [ ] T172 [P] Verify all user-facing screens render correctly in RTL Arabic at mobile (375px), tablet (768px), and desktop (1280px) viewports
- [ ] T173 [P] Security: configure CORS, rate limiting middleware for OTP endpoints, input sanitization in aspnet-core/src/MshNawy.HttpApi.Host/
- [ ] T174 Seed initial FeePolicy (entry 1%, payment 3%, exit 5%) and admin user via aspnet-core/src/MshNawy.DbMigrator/ data seeder
- [ ] T175 Run quickstart.md validation: verify full local setup from scratch following quickstart steps
- [ ] T180 [P] Integration tests for concurrency: concurrent orders exceeding available balance (second must fail atomically), concurrent withdrawal + investment (pending withdrawal reduces effective available balance), optimistic concurrency stamp verification in aspnet-core/test/MshNawy.Application.Tests/Concurrency/ConcurrencyIntegrationTests.cs

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 — BLOCKS all user stories
- **US1 Onboarding (Phase 3)**: Depends on Phase 2 — MVP entry point
- **Remediation (Phase 3b)**: Depends on Phase 3 — BLOCKS Phase 4. Fixes constitution violations, rename remnants, guard wiring, idempotency middleware.
- **US2 Deposits (Phase 4)**: Depends on Phase 3b. Soft dependency on US1 (KYC guard). Requires idempotency middleware (T187).
- **US3 Offerings (Phase 5)**: Depends on Phase 3b. No dependency on US1/US2
- **US4 Subscription (Phase 6)**: Depends on Phase 2. Requires US2 (balance) + US3 (offerings) entities to exist
- **US5 Portfolio (Phase 7)**: Depends on Phase 2. Requires US4 (holdings created by order settlement)
- **US6 Property Sale (Phase 8)**: Depends on Phase 2. Requires US4 (holdings exist from order settlement). Soft dependency on US5 (portfolio view).
- **US6b Withdrawal (Phase 9)**: Depends on Phase 2. Soft dependency on US2 (wallet UI)
- **US7 Support (Phase 10)**: Depends on Phase 2. No dependency on other stories
- **Notifications (Phase 11)**: Depends on all user story domain managers being implemented
- **Admin (Phase 12)**: Admin infrastructure only (auth, permissions, audit, dashboard, fee policy, lazy module). Per-story admin screens are built within their respective story phases. Depends on Phase 2; can start after US1 admin tasks exist.
- **Polish (Phase 13)**: Depends on all previous phases

### User Story Dependencies

```
Phase 1 (Setup)
    ↓
Phase 2 (Foundational) ←── BLOCKS ALL
    ↓
Phase 3 (US1 Onboarding) ✅
    ↓
Phase 3b (Remediation) ←── BLOCKS Phase 4+
    ↓
    ├── US2 (Deposits) ───────────────────────────────┐    │
    ├── US3 (Offerings) ──────────────────────────┐   │    │
    │                                             │   │    │
    │                                             ↓   ↓    │
    ├── US4 (Subscription) ← needs US2 + US3 ────┐   │    │
    │                                             │   │    │
    │                                             ↓   │    │
    ├── US5 (Portfolio) ← needs US4 holdings      │   │    │
    │                                             ↓   │    │
    ├── US6 (Property Sale) ← needs US4 holdings           │   │    │
    │                                             │   │    │
    ├── US6b (Withdrawal) ← independent           │   │    │
    ├── US7 (Support) ← independent               │   │    │
    │                                             │   │    │
    ↓                                             │   │    │
Phase 11 (Notifications) ← needs all managers    │   │    │
    ↓                                             │   │    │
Phase 12 (Admin) ← needs all admin services      │   │    │
    ↓                                             │   │    │
Phase 13 (Polish) ← needs all                    │   │    │
```

### Within Each User Story

1. Contracts (DTOs) — first, defines the interface
2. Mocks (MSW handlers) — enables frontend development
3. Angular UI — built against mocks
4. Storybook stories — validates all states
5. Backend domain — entities + domain services
6. Backend application + API — app services + controllers
7. Tests — validates domain logic
8. Admin screens — operational support

### Parallel Opportunities

**Cross-story parallelism** (after Phase 2):
- US1, US2, US3 can all start in parallel (no mutual dependencies)
- US6b and US7 can start anytime after Phase 2
- Within each story, contracts and mocks are independent of other stories

**Within-story parallelism** (marked [P]):
- Contract DTOs (backend) and TypeScript interfaces (frontend) in parallel
- Storybook stories in parallel with backend implementation
- Multiple test files in parallel

---

## Parallel Examples

### After Phase 2: Launch US1 + US2 + US3 in parallel

```
Developer A: T027→T029→T030→T031→T032→T033 (US1 frontend)
Developer B: T046→T048→T049→T050→T051→T052 (US2 frontend)
Developer C: T063→T065→T066→T067→T068→T069→T070 (US3 frontend)
```

### Within US4: Parallel contract + mock creation

```
Parallel: T080 (backend DTOs) + T081 (frontend interfaces)
Then: T082 (MSW handlers — needs interfaces)
```

---

## Implementation Strategy

### MVP First (US1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL)
3. Complete Phase 3: US1 — Onboarding & KYC
4. **STOP and VALIDATE**: Test onboarding flow end-to-end with mocks
5. Demo with mock data

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. US1 (Onboarding) → Test independently → Demo (MVP!)
3. US2 (Deposits) → Wallet functional → Demo
4. US3 (Offerings) → Browsing live → Demo
5. US4 (Subscription) → Full investment flow → Demo
6. US5 (Portfolio) → Investor dashboard → Demo
7. US6 + US6b (Exit + Withdrawal) → Cash-out works → Demo
8. US7 (Support) → Ticket system → Demo
9. Notifications + Admin + Polish → Production ready

### Parallel Team Strategy (3 developers)

1. All three complete Setup + Foundational together
2. Once Foundational is done:
   - **Dev A**: US1 (Onboarding) → US4 (Subscription) → US5 (Portfolio)
   - **Dev B**: US2 (Deposits) → US6b (Withdrawal) → US6 (Exit)
   - **Dev C**: US3 (Offerings) → US7 (Support) → Notifications
3. All converge on Admin + Polish

---

## Notes

- [P] tasks = different files, no dependencies on incomplete tasks
- [Story] label maps task to specific user story for traceability
- Each user story independently completable and testable with mocks
- Frontend-first: build UI against MSW before backend implementation
- All monetary values in piasters (long) — frontend converts to EGP for display
- Every financial operation needs ledger entries — reference LedgerService
- State machines use domain methods — no direct status property sets
- Commit after each task or logical group (≤400 LOC per PR)
- Stop at any checkpoint to validate story independently

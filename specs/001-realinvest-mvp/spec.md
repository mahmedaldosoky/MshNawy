# Feature Specification: MshNawy (مش ناوي) — Egyptian Fractional Real Estate Investment Platform

**Feature Branch**: `001-realinvest-mvp`
**Created**: 2026-02-28 | **Updated**: 2026-03-08
**Status**: Draft
**Product Name**: MshNawy (مش ناوي)
**Input**: Build a web application named MshNawy (مش ناوي) for Egyptians only, Arabic-only RTL, that provides fractional/share-based real estate investing with a realistic, dynamic financial experience.

## User Scenarios & Testing *(mandatory)*

### User Story 1 — Onboarding & KYC Verification (Priority: P1)

A new Egyptian user discovers RealInvest and wants to create an account so they can start investing. They register with their Egyptian mobile number, verify it via OTP, then complete KYC by submitting their national ID details and photos. Until KYC is approved, they cannot deposit or invest.

**Why this priority**: Without identity verification, no financial activity can occur. This is the entry gate for the entire platform and a regulatory requirement.

**Independent Test**: A user can register, receive OTP, verify their phone, submit KYC documents, and see their KYC status update — all without any other features being active.

**Acceptance Scenarios**:

1. **Given** an unregistered Egyptian mobile number, **When** the user enters it on the registration screen, **Then** the system sends a 6-digit OTP to that number and displays a verification input screen (all in Arabic, RTL).
2. **Given** a valid OTP is entered within 3 minutes, **When** the user submits it, **Then** the system creates the account and lands the user on the home screen with a KYC prompt.
3. **Given** an incorrect OTP is entered, **When** the user submits it, **Then** the system displays an Arabic error message and allows retry (up to 5 attempts per 15-minute window).
4. **Given** a registered user without approved KYC, **When** the user navigates to deposit or investment screens, **Then** the system blocks access and redirects to the KYC completion flow.
5. **Given** a user on the KYC screen, **When** they fill in their full name (Arabic), date of birth, national ID number (14 digits), upload front and back photos of their national ID, and upload a selfie, **Then** the system validates the inputs, stores the submission, and transitions KYC status to "Submitted".
6. **Given** KYC status is "Submitted", **When** a back-office reviewer approves it, **Then** the user sees status change to "Approved" and gains access to deposit and investment features.
7. **Given** KYC is rejected, **When** the user views their KYC status, **Then** they see the rejection reason in Arabic and can resubmit with corrected information (status moves to "Needs Resubmission" → "Submitted" on resubmission).

---

### User Story 2 — Wallet Deposit via Settlement-Based Methods (Priority: P1)

A KYC-approved user wants to add funds to their RealInvest wallet so they can invest. They choose one of three deposit methods (InstaPay, Vodafone Cash, or Bank Transfer), follow the displayed instructions, upload proof of transfer, and wait for back-office settlement approval.

**Why this priority**: Deposits are the prerequisite for investing. Without funds in the wallet, the core investment flow cannot function. This also establishes the double-entry ledger foundation.

**Independent Test**: A KYC-approved user can initiate a deposit, see payment instructions with a unique reference, upload proof, and observe the deposit move through its lifecycle — all without investment features being active.

**Acceptance Scenarios**:

1. **Given** a KYC-approved user on the deposit screen, **When** they select "InstaPay" as the deposit method, **Then** the system displays the platform's InstaPay account/IPA address and a unique reference code for this deposit, all in Arabic.
2. **Given** a KYC-approved user on the deposit screen, **When** they select "Vodafone Cash", **Then** the system displays the platform's Vodafone Cash wallet number and a unique reference code.
3. **Given** a KYC-approved user on the deposit screen, **When** they select "Bank Transfer", **Then** the system displays full bank account details (bank name, account number, IBAN) and a unique reference code.
4. **Given** the user has made the external transfer, **When** they upload proof (screenshot or receipt image) and submit, **Then** the deposit status changes to "Pending Review" and appears in the user's transaction history.
5. **Given** a back-office reviewer approves the deposit, **When** the approval is processed, **Then** the deposit amount is posted to the user's "Available" balance via double-entry ledger entries (debit: settlement account, credit: user available account), and the user sees the updated balance.
6. **Given** a back-office reviewer rejects the deposit, **When** the rejection is processed, **Then** the deposit status changes to "Rejected" with a reason visible to the user in Arabic, and no balance change occurs.
7. **Given** a deposit is approved, **When** the user views their wallet, **Then** the wallet displays three separate balances: Available, Reserved, and Invested — with the deposit reflected only in Available.

---

### User Story 3 — Browse Offerings with Dynamic Financial Projections (Priority: P1)

An investor wants to explore available real estate investment opportunities and understand potential returns before committing funds. Each offering displays dynamically computed projections based on the property's financial model, not static percentages.

**Why this priority**: The offerings catalog with realistic financial projections is the core value proposition. Investors need transparent, model-backed numbers to make informed decisions. This differentiates RealInvest from competitors showing static ROI claims.

**Independent Test**: A user can browse offerings, view dynamic projections under different scenarios, see fee breakdowns, and explore payment timelines — all without needing to actually invest.

**Acceptance Scenarios**:

1. **Given** a logged-in user on the offerings page, **When** the page loads, **Then** the system displays a list of available investment opportunities, each showing: property name, location, share price, total shares, available shares, projected annual return range, risk level, and a brief description — all in Arabic with EGP formatting.
2. **Given** a user viewing an offering detail page, **When** the page loads, **Then** the system computes and displays: projected annual return range (conservative/base/optimistic), projected distribution schedule (timeline of expected rental income payments), projected exit value range, total fee impact breakdown, and payment plan structure.
3. **Given** a user on the offering detail page, **When** they toggle between Conservative, Base, and Optimistic scenarios, **Then** the projected returns, distribution timeline chart, and exit value range recalculate dynamically based on different assumption inputs (occupancy rate, appreciation rate).
4. **Given** any offering displayed in the UI, **When** inspected, **Then** all financial numbers are backed by a computational model with explicit inputs: property price, number of shares, payment plan schedule, expected rent, occupancy assumptions, appreciation assumptions, exit date, and applicable fee policy. No static/hard-coded return percentages exist.
5. **Given** a user viewing an offering, **When** they expand the fee breakdown section, **Then** the system displays: entry fee (1% of share value, charged with down payment), per-payment fee (3% of each installment), and exit fee (5% of resale price split as 2.5% brokerage + 2.5% platform profit) — with all amounts in EGP.
6. **Given** an offering with a payment plan, **When** the user views the timeline section, **Then** a visual chart shows the payment schedule (down payment + installments) alongside expected distribution dates and amounts, with a "net return after fees" summary.
7. **Given** any offering page, **When** displayed, **Then** a visible disclaimer states that all projections are estimates and not guaranteed returns.

---

### User Story 4 — Subscribe to an Investment Offering (Priority: P1)

An investor with available funds wants to purchase shares in a real estate offering. They go through the subscription flow which includes a knowledge check, order creation, fund reservation, and settlement. Fees are applied transparently.

**Why this priority**: The investment subscription is the core revenue-generating transaction. It ties together the wallet, ledger, fee engine, and offering systems into the primary business flow.

**Independent Test**: A user with available balance can select an offering, complete the knowledge check, create an order, see funds reserved, and observe the order settle with shares issued — verifying the complete ledger trail.

**Acceptance Scenarios**:

1. **Given** a first-time investor selecting "Subscribe" on an offering, **When** the subscription flow starts, **Then** the system presents a mandatory knowledge check questionnaire (investment risk awareness) in Arabic that MUST be completed before proceeding.
2. **Given** a returning investor whose previous knowledge check covered the same or lower risk level, **When** they subscribe to a same-or-lower-risk offering, **Then** the knowledge check is skipped. If the new offering has a higher risk level, the knowledge check is re-presented.
3. **Given** a user who passed the knowledge check, **When** they confirm the number of shares to purchase, **Then** the system displays a complete order summary showing: share price, number of shares, entry fee (1% of share value), total cost (shares + fee), payment plan schedule (if applicable), and a breakdown of how fees will apply to each installment — all in Arabic/EGP.
4. **Given** the user confirms the order, **When** the system processes it, **Then** the order status moves to "Reserved": the total cost (including entry fee) is atomically moved from "Available" to "Reserved" balance via double-entry ledger entries, using an idempotency key to prevent duplicate charges.
5. **Given** an order in "Reserved" status, **When** the settlement process approves it, **Then** the reserved amount moves to "Invested" balance via ledger entries, shares/units are issued to the user's portfolio, and the order status becomes "Settled".
6. **Given** an order settlement fails, **When** the failure is processed, **Then** the reserved funds are returned to "Available" via compensating ledger entries, and the user sees the order marked as "Failed" with an Arabic explanation.
7. **Given** a user without sufficient available balance, **When** they attempt to subscribe, **Then** the system blocks the order with an Arabic message indicating insufficient funds and suggesting a deposit.

---

### User Story 5 — Portfolio View & Statements (Priority: P2)

An investor wants to view their holdings, track their investment performance, see fee history, and export a statement for their records.

**Why this priority**: Portfolio visibility builds trust and retention. While not blocking the investment flow itself, investors expect immediate visibility into their holdings after purchase.

**Independent Test**: A user with issued shares can view their portfolio dashboard showing holdings, cost basis, fees paid, projected values, and activity history. They can export a statement.

**Acceptance Scenarios**:

1. **Given** a user with one or more investments, **When** they navigate to the portfolio page, **Then** they see a summary: total invested amount, total projected value range (current), total fees paid to date, and number of active holdings — all in Arabic/EGP.
2. **Given** a user viewing portfolio details, **When** they expand a specific holding, **Then** they see: offering name, number of shares owned, cost basis (total paid including fees), paid-to-date amount, fees paid-to-date (entry + payment fees), projected current value range, and a list of all activity (purchase, payments, distributions) with dates.
3. **Given** a user on the portfolio page, **When** they tap "Export Statement", **Then** the system generates an Arabic-formatted HTML document containing: the user's name, statement period, holdings summary, transaction history, and fee breakdown — suitable for printing or saving.
4. **Given** a user's portfolio, **When** the offering's underlying model inputs change (e.g., updated occupancy data), **Then** the projected value range in the portfolio updates dynamically to reflect the new calculations.

---

### User Story 6 — Property Sale & Proceeds Distribution (Priority: P2)

Investors exit their holdings when the underlying property is actually sold. There is no individual early exit — this is a closed-end model. A property sale can be triggered by a majority investor vote (>50% of shares), by reaching the offering's maturity date, or by a platform decision. When the property sells, all holders receive their pro-rata share of actual sale proceeds minus the exit fee.

**Why this priority**: Exit capability is essential for investor confidence — knowing the exit mechanism is transparent and fair builds trust. The closed-end model protects both the platform and investors from pricing-mismatch risk. However, it is secondary to the purchase flow.

**Independent Test**: A user with holdings can view their offering's sale vote status, cast a vote to sell, see the property sale lifecycle, and upon sale completion see net proceeds credited to their available balance.

**Acceptance Scenarios**:

1. **Given** a user with shares in an offering, **When** they view the holding detail, **Then** the system displays: the offering's maturity date, current sale vote status (percentage of shares that have voted to sell), and the exit fee policy (2.5% brokerage + 2.5% platform profit = 5% total) — all in Arabic/EGP.
2. **Given** a user with shares, **When** they tap "Vote to Sell" on a holding, **Then** their vote is recorded and the sale vote percentage updates. A user can also withdraw their vote before the threshold is reached.
3. **Given** the sale vote reaches >50% of total shares, **When** the threshold is crossed, **Then** the system notifies the admin that a majority vote to sell has been reached for the offering and transitions the offering to "Sale Initiated" status.
4. **Given** an offering with "Sale Initiated" status, **When** the admin lists the property on the market and records the actual sale price, **Then** the system calculates pro-rata proceeds for each holder: (shares owned / total shares) * actual sale price, minus exit fee (5%), and displays the breakdown.
5. **Given** the admin confirms the property sale settlement, **When** settlement completes, **Then**: the exit fee is deducted via ledger entries, net proceeds are posted to each holder's "Available" balance, all holdings for the offering are deactivated, remaining installments are cancelled, and the offering status becomes "Settled".
6. **Given** a property sale is in progress, **When** any holder views their holding, **Then** they see the current sale status (Sale Initiated → Listed → Sold → Distributing → Settled) and, once the sale price is recorded, their estimated net proceeds.
7. **Given** an offering that has reached its maturity date without a majority vote, **When** the maturity date arrives, **Then** the admin is notified and may initiate a property sale at their discretion (platform-initiated sale).

---

### User Story 6b — Wallet Withdrawal (Priority: P2)

A user with Available balance wants to withdraw funds to their personal bank account or Vodafone Cash wallet so they can access their money outside the platform.

**Why this priority**: Withdrawal is the complement to deposit and essential for investor confidence. Without a cash-out mechanism, investors cannot realize returns from exits or unused deposits.

**Independent Test**: A KYC-approved user with Available balance can request a withdrawal via Bank Transfer or Vodafone Cash, see the applicable fees, track the request status, and upon back-office approval see their Available balance decrease accordingly.

**Acceptance Scenarios**:

1. **Given** a KYC-approved user with Available balance, **When** they navigate to the withdrawal screen, **Then** they see two withdrawal methods: Bank Transfer and Vodafone Cash — all in Arabic.
2. **Given** a user selecting "Bank Transfer", **When** they enter the withdrawal amount and their bank details (bank name, account holder name, account number/IBAN), **Then** the system validates the amount does not exceed Available balance and displays a confirmation summary with no additional fee.
3. **Given** a user selecting "Vodafone Cash", **When** they enter the withdrawal amount and their Vodafone Cash wallet number, **Then** the system validates the amount, displays a confirmation summary showing a flat 5 EGP processing fee, and shows the net amount the user will receive.
4. **Given** the user confirms the withdrawal, **When** submitted, **Then** the withdrawal amount (plus fee if Vodafone Cash) is atomically moved from "Available" to "Pending Withdrawal" via double-entry ledger entries using an idempotency key, and the request status becomes "Pending Review".
5. **Given** a back-office reviewer approves the withdrawal, **When** the approval is processed, **Then** the pending amount is settled via ledger entries, the withdrawal status becomes "Completed", and the user sees the updated balance.
6. **Given** a back-office reviewer rejects the withdrawal, **When** the rejection is processed, **Then** the pending amount is returned to "Available" via compensating ledger entries, the status becomes "Rejected" with an Arabic reason visible to the user.
7. **Given** a user with a pending withdrawal, **When** they view the transaction history, **Then** they see the withdrawal with status (Pending Review → Processing → Completed / Rejected) and the fee applied (if Vodafone Cash).

---

### User Story 7 — Support & Complaints (Priority: P3)

A user encounters an issue or has a question and wants to submit a support ticket with attachments, then track its resolution.

**Why this priority**: Support is important for user trust but not blocking for the core investment flows. Users can rely on external channels initially.

**Independent Test**: A user can create a ticket with text and attachments, view ticket status, and see updates from support staff.

**Acceptance Scenarios**:

1. **Given** a logged-in user, **When** they navigate to Support and tap "New Ticket", **Then** they see a form with: subject, category dropdown, description text area, and file attachment option — all in Arabic.
2. **Given** a user filling the support form, **When** they attach up to 3 images/documents and submit, **Then** the ticket is created with a unique reference number and SLA timer starts.
3. **Given** a user with open tickets, **When** they view the support section, **Then** they see all tickets with: reference number, subject, status (Open → In Progress → Resolved → Closed), creation date, and SLA status indicator.
4. **Given** a support agent responds to a ticket, **When** the user views the ticket, **Then** they see the response thread and can reply with additional information or attachments.

---

### User Story 8 — Mock-First Frontend Experience (Priority: P1)

The development team needs to build and validate the entire frontend experience using mock data and simulated backend state transitions before any backend is implemented.

**Why this priority**: Frontend-first delivery is a core development methodology requirement. The frontend must be fully functional with mocks to validate UX before backend investment begins.

**Independent Test**: The entire frontend application runs end-to-end using local mock data files and a mock API layer. All user flows (onboarding, deposit, withdrawal, invest, portfolio, exit) can be demonstrated with deterministic, consistent data.

**Acceptance Scenarios**:

1. **Given** the frontend application with mock mode enabled, **When** a developer runs it locally, **Then** all screens render with deterministic Arabic mock data (same data every run) — including user profiles, offerings, wallet balances, and transaction histories.
2. **Given** mock mode, **When** a user goes through the OTP flow, **Then** the mock layer accepts any 6-digit code and simulates successful verification.
3. **Given** mock mode, **When** a user submits a deposit with proof, **Then** the mock layer simulates the deposit lifecycle (Created → Pending Review → Approved → Posted) with configurable delays or instant transitions for testing.
4. **Given** mock mode, **When** a user subscribes to an offering, **Then** the mock layer simulates: fund reservation, settlement approval, and share issuance — with all ledger entries visible in the transaction history.
5. **Given** mock mode offerings data, **When** the projection engine runs, **Then** all financial numbers (returns, distributions, fees) are computed from the same seeded model inputs and produce identical results across runs.

---

### Edge Cases

- **OTP expiration**: OTP expires after 3 minutes. User must request a new one. Rate limit: 5 requests per 15-minute window per phone number; after that, the number is locked for 30 minutes.
- **KYC image quality**: If uploaded images are unreadable (too blurry, wrong format), the back-office reviewer marks KYC as "Needs Resubmission" with a specific reason.
- **Deposit proof mismatch**: If the uploaded proof does not match the reference code or amount, the deposit is rejected with a reason.
- **Insufficient balance for investment**: Order is blocked pre-submission. The user sees a clear Arabic message and a link to the deposit screen.
- **Concurrent orders**: If a user attempts two subscriptions simultaneously that would exceed their available balance, the second order fails atomically (no partial reservation).
- **Order settlement failure**: Reserved funds are returned to Available via compensating ledger entries. No funds are lost in any failure scenario.
- **Network interruption during order**: Idempotency keys ensure re-submission of the same order does not create duplicates. The user sees the existing order status upon retry.
- **Fee policy change mid-investment**: Existing investments retain the fee policy that was active at the time of purchase. New investments use the current policy.
- **Zero available shares**: If an offering is fully subscribed, the "Subscribe" button is disabled with an Arabic message explaining the offering is closed.
- **Property sale on an offering still in payment plan**: When a property sale settles, all remaining unpaid installments for all holders of that offering are cancelled (status → Cancelled). The exit fee is calculated on the actual sale price. Each holder's net proceeds are: (shares owned / total shares) * actual sale price - exit fee. If a holder has overpaid relative to their pro-rata share, the excess is included in their proceeds.
- **Missed installment**: If a user's Available balance is insufficient on the installment due date, the installment enters a grace period (default 7 days). The user sees an Arabic warning with the overdue amount and deadline. After grace period expiry without payment, the investment is flagged for back-office review (manual resolution — e.g., contact user, restructure, or liquidate).
- **Multiple overdue installments**: If a user has more than one overdue installment across different offerings, all are visible in a single "Overdue Payments" section in the wallet/portfolio view.
- **Withdrawal exceeds available balance**: The withdrawal is blocked pre-submission. The user sees an Arabic message indicating insufficient available funds.
- **Withdrawal with Vodafone Cash where balance < amount + 5 EGP fee**: The system blocks the request and shows the maximum withdrawable amount after fee deduction.
- **Concurrent withdrawal and investment**: If a user has a pending withdrawal and attempts an investment (or vice versa), the system accounts for the pending withdrawal amount when checking available balance to prevent over-commitment.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST support user registration and login exclusively via Egyptian mobile phone numbers (format: +20 followed by 10 digits) with OTP verification.
- **FR-002**: The system MUST enforce KYC completion (national ID number, full Arabic name, date of birth, front/back ID photos, selfie) before allowing any deposit or investment activity.
- **FR-003**: The system MUST implement a KYC state machine with states: Draft → Submitted → Under Review → Approved / Rejected (with reason) / Needs Resubmission, with back-office transitions.
- **FR-004**: The system MUST maintain a wallet for each user with four distinct balances derived from the underlying double-entry ledger: **Available** (free to invest or withdraw), **Reserved** (earmarked for a pending order), **Invested** (settled into active holdings), and **PendingWithdrawal** (atomically moved from Available while a withdrawal request awaits back-office approval). Each balance is computed from ledger entry summations — no stored balance columns.
- **FR-005**: Every monetary movement MUST be recorded as double-entry ledger entries (debit + credit). Approved/posted ledger entries MUST be immutable; corrections MUST use compensating entries only.
- **FR-006**: The system MUST support three deposit methods: InstaPay transfer, Vodafone Cash transfer, and Bank transfer — each displaying method-specific payment details and a unique reference code.
- **FR-007**: Deposits MUST follow the lifecycle: Created → Pending Review → Approved/Rejected → Posted, with back-office settlement approval required before funds are posted to the user's Available balance.
- **FR-008**: The system MUST display investment offerings with dynamically computed financial projections (not static percentages) including: projected annual return range, distribution schedule, exit value range, fee impact, and risk level.
- **FR-009**: Each offering MUST have a computational cashflow model with explicit inputs: property price, number of shares, payment plan schedule, expected rent, occupancy assumptions, appreciation assumptions, exit date, and fee policy.
- **FR-010**: The system MUST support three scenario modes for projections: Conservative, Base, and Optimistic — each with different assumption parameters that dynamically update all computed outputs.
- **FR-011**: The system MUST enforce a mandatory knowledge check and risk acknowledgement before a user's first investment. The knowledge check MUST be re-presented when the user invests in an offering with a higher risk level than previously acknowledged.
- **FR-012**: The investment subscription flow MUST follow the sequence: Create Order → Reserve Funds (Available → Reserved) → Submit → Settled (Reserved → Invested, units issued) — with compensating ledger entries on failure (Reserved → Available).
- **FR-013**: All financial operations (deposits, withdrawals, order reservations, settlements) MUST be atomic and idempotent, using client-generated idempotency keys to prevent duplicate processing.
- **FR-027**: Offerings MAY define a payment plan consisting of a down payment percentage (paid at subscription) followed by equal monthly installments over a defined period. The admin sets the down payment percentage, number of installments, and installment start date per offering.
- **FR-028**: When an installment is due, the system MUST attempt to deduct the installment amount (plus 3% per-payment fee) from the user's Available balance via double-entry ledger entries. If insufficient funds, the installment enters a grace period.
- **FR-029**: The grace period for a missed installment MUST be configurable per offering (default: 7 days). During the grace period, the user sees an Arabic notification indicating the overdue amount and deadline. After grace period expiry, the investment is flagged for back-office review.
- **FR-030**: For offerings without a payment plan, the full share price (plus entry fee) is charged upfront at subscription.
- **FR-014**: The system MUST implement a configurable, versioned fee policy with: entry fee (1% of share value with down payment), per-payment fee (3% of each installment), and exit fee (5% of resale price split 2.5% brokerage + 2.5% platform profit).
- **FR-015**: Fee breakdowns MUST be displayed to the user in Arabic before any confirmation step (subscription, exit). Fees MUST appear as separate line items in ledger entries.
- **FR-016**: The portfolio view MUST show per-holding: shares owned, cost basis, paid-to-date, fees paid-to-date, projected value range, and chronological activity history.
- **FR-017**: Users MUST be able to export an Arabic-formatted statement (initially as HTML) containing holdings summary, transaction history, and fee breakdown.
- **FR-018**: The system MUST implement a closed-end exit model where investors exit only when the underlying property is sold. Property sales are triggered by: (a) majority investor vote (>50% of shares vote to sell), (b) offering maturity date reached, or (c) platform decision. Upon property sale settlement, actual sale proceeds are distributed pro-rata to all holders minus exit fee (5%), with net proceeds posted to each holder's Available balance. Individual early exit requests are NOT supported.
- **FR-019**: The system MUST provide in-app Arabic support ticketing with: subject, category, description, file attachments (up to 3), unique reference number, SLA tracking, and threaded conversation between user and support agent.
- **FR-024**: The system MUST support wallet withdrawals via two methods: Bank Transfer (no additional fee) and Vodafone Cash (flat 5 EGP processing fee deducted from the withdrawal amount).
- **FR-025**: Withdrawals MUST follow the lifecycle: Created → Pending Review → Processing → Completed/Rejected, with back-office manual settlement approval required.
- **FR-026**: Withdrawal amounts (plus applicable fees) MUST be atomically reserved from "Available" balance via double-entry ledger entries with an idempotency key upon submission. Rejected withdrawals MUST return funds via compensating entries.
- **FR-034**: The system MUST provide a back-office admin panel with the following operational screens: KYC review (list pending submissions, view documents, approve/reject with reason), deposit approval (list pending deposits, view proof images, approve/reject), withdrawal approval (list pending withdrawals, approve/reject), order settlement (list reserved orders, settle/fail), property sale management (view offerings with majority vote reached or maturity due, initiate sale, record actual sale price, settle distribution to all holders), support ticketing (view/respond to tickets, update status), and offering management (create/edit offerings with all model inputs including maturity date, open/close offerings).
- **FR-035**: Admin panel users MUST be authenticated separately from investor users. Admin actions MUST be audit-logged with the admin user ID, action type, target entity, and timestamp.
- **FR-036**: The admin panel MAY support both Arabic and English. It is not required to be Arabic-only as it is an internal tool.
- **FR-031**: The system MUST provide an in-app notification center accessible via a bell icon in the navigation bar, showing unread count badge and a chronological list of all status-change alerts in Arabic.
- **FR-032**: Notifications MUST be generated for all user-facing status transitions including: KYC status changes, deposit posted/rejected, withdrawal completed/rejected, order settled/failed, installment due/overdue, property sale status updates (vote threshold reached, sale initiated, proceeds distributed), and support ticket responses.
- **FR-033**: Each notification MUST display: timestamp, Arabic description of the event, link to the relevant screen (e.g., tap a deposit notification to view the deposit detail), and read/unread status.
- **FR-020**: The entire UI MUST be Arabic-only with RTL layout. All numbers MUST display in EGP with Arabic locale formatting. No English strings in any user-facing screen.
- **FR-021**: The frontend MUST be fully functional using a mock API layer and deterministic mock data files, enabling end-to-end demonstration of all user flows without a live backend.
- **FR-022**: Each offering MUST display a visible disclaimer stating that projections are estimates and not guaranteed returns.
- **FR-023**: Fee policy MUST support effective-date versioning so that existing investments retain the fee terms active at the time of purchase.

### Key Entities

- **User**: Egyptian mobile number, KYC status, KYC documents (name, DOB, national ID number, ID images, selfie), account creation date.
- **Wallet**: Associated with one user. Contains derived balances (Available, Reserved, Invested) computed from ledger entries.
- **Ledger Entry**: Immutable record containing: timestamp, entry type, debit account, credit account, amount (in piasters), idempotency key, reference entity (deposit/order/exit), description.
- **Deposit**: Method (InstaPay/Vodafone Cash/Bank Transfer), reference code, amount, proof image, status (Created → Pending Review → Approved/Rejected → Posted), reviewer notes.
- **Offering**: Property details (name, location, description, images), financial model inputs (property price, share count, payment plan structure [down payment %, installment count, installment start date, grace period days], rent assumptions, occupancy, appreciation, maturity date), risk level, available shares count, sale vote threshold (default >50%), status (Draft/Open/Closed/SaleInitiated/Settled).
- **Installment**: Associated with an investment order, due date, amount, fee amount (3%), status (Upcoming → Due → Paid → Overdue → Grace Period → Flagged), payment ledger entry reference.
- **Fee Policy**: Version, effective date range, entry fee percentage, payment fee percentage, exit fee percentage (with brokerage/platform split), associated offering or global default.
- **Investment Order**: User, offering, share count, total cost, fee breakdown, idempotency key, status (Created → Reserved → Submitted → Settled → Units Issued / Failed), associated ledger entries.
- **Holding**: User, offering, shares owned, cost basis, fees paid, acquisition date, linked investment order(s).
- **Sale Vote**: User, offering, vote direction (sell/hold), timestamp. One vote per user per offering. Withdrawable before threshold.
- **Property Sale**: Offering, trigger type (MajorityVote/Maturity/PlatformDecision), actual sale price, status (Initiated → Listed → Sold → Distributing → Settled / Cancelled), settlement date.
- **Sale Distribution**: Property sale, user, shares held, gross proceeds, exit fee, net proceeds, ledger entry reference.
- **Support Ticket**: User, reference number, subject, category, description, attachments, status (Open → In Progress → Resolved → Closed), SLA deadline, conversation thread.
- **Knowledge Check Record**: User, risk level acknowledged, completion date, associated offering risk level.
- **Notification**: User, event type, event reference (entity type + ID), Arabic message text, link target (screen/route), read/unread status, timestamp.
- **Withdrawal**: User, method (Bank Transfer/Vodafone Cash), amount, fee (0 or 5 EGP), net amount, destination details (bank account or Vodafone wallet number), idempotency key, status (Created → Pending Review → Processing → Completed / Rejected), reviewer notes.

## Clarifications

### Session 2026-02-28

- Q: Can users withdraw funds from their Available balance to an external account? What methods are supported? → A: Yes. Bank Transfer (no extra fee) and Vodafone Cash (flat 5 EGP fee). Back-office manual settlement.
- Q: How do payment plan installments work for offerings? → A: Fixed schedule — admin-defined down payment % + equal monthly installments. Missed installments enter a configurable grace period (default 7 days), then flagged for back-office review.
- Q: How are users notified of status changes (KYC, deposits, orders, installments)? → A: In-app notification center only for MVP (bell icon, unread badge, chronological list). SMS and email notifications deferred to production.
- Q: What is the scope of the back-office admin panel in MVP? → A: Full operational admin covering all settlement actions: KYC review, deposit/withdrawal approval, order settlement, exit processing, support ticketing, and offering management. Simple list+detail screens. Audit-logged.

## Assumptions

- **OTP delivery**: OTP messages are sent via a third-party SMS gateway. For MVP, the mock layer simulates OTP delivery; actual SMS integration is deferred to backend implementation.
- **KYC verification**: KYC approval is performed manually by back-office staff. No automated ID verification service is integrated in MVP.
- **Settlement**: All settlement (deposit approval, order settlement, property sale processing, proceeds distribution) is manual back-office action. No automated payment provider integration in MVP.
- **Risk levels**: Offerings have predefined risk levels (Low, Medium, High) assigned by the platform administrators. The knowledge check content maps to these three risk tiers.
- **Single currency**: All transactions are in Egyptian Pounds (EGP). No multi-currency support.
- **Single language**: Arabic only. No internationalization framework needed beyond Arabic locale.
- **Statement format**: MVP exports HTML statements. PDF export is a planned future enhancement. Statements include only posted/settled transactions (not pending or in-progress ones). The user selects a date range for the statement period.
- **File uploads**: Images for KYC and deposit proof are uploaded and stored securely. Maximum file size: 5 MB per image. Accepted formats: JPEG, PNG.
- **SLA for support**: Default SLA is 48 hours for first response. For MVP, the SLA deadline is a hard-coded constant (48 hours). Admin-configurable SLA per category is a post-MVP enhancement.
- **Concurrency**: The system handles up to 100 concurrent users for MVP. Performance optimization for higher scale is post-MVP.
- **Back-office UI**: The admin panel covers all operational actions referenced in user-facing flows: KYC review, deposit/withdrawal approval, order settlement, exit processing, support ticketing, and offering management. Screens are simple list+detail views. It is not public-facing and supports both Arabic and English.
- **No secondary market**: Users cannot trade shares with each other in MVP. Exit is exclusively via property sale (closed-end model). No individual early exit or share transfer is supported.
- **Notifications**: MVP uses in-app notification center only. SMS notifications and email notifications are planned for production but not included in MVP scope.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users complete the registration + OTP verification flow in under 2 minutes (measured from entering phone number to landing on home screen).
- **SC-002**: Users complete KYC submission in under 5 minutes (measured from starting KYC form to submission confirmation).
- **SC-003**: 100% of deposit, investment, and exit transactions produce correct double-entry ledger entries that balance to zero across all accounts when audited.
- **SC-004**: Users can browse offerings, toggle scenarios, and view dynamic projections with page load and recalculation completing in under 2 seconds.
- **SC-005**: The subscription flow (from "Subscribe" to order confirmation) completes in under 3 steps and 90 seconds for a returning user who has passed the knowledge check.
- **SC-006**: All financial projections produced by the system are reproducible — identical inputs and fee policy version yield identical outputs across sessions and devices.
- **SC-007**: 100% of user-facing screens render correctly in RTL Arabic layout at mobile (375px), tablet (768px), and desktop (1280px) viewport widths.
- **SC-008**: The frontend application runs fully end-to-end using mock data, enabling complete demonstrations of all user flows (onboarding, deposit, withdrawal, invest, portfolio, sale vote, property sale, support) without any backend dependency.
- **SC-009**: Fee breakdowns are visible to users before every confirmation step across subscriptions (order summary) and property sale settlement (distribution preview).
- **SC-010**: Users can view portfolio holdings, activity history, and export an Arabic statement within 3 taps from the home screen.
- **SC-011**: Support tickets receive a visible SLA indicator and the system tracks 95% of tickets against the configured SLA deadline.
- **SC-012**: No financial operation (deposit, withdrawal, order, exit) results in lost or duplicated funds — verified by ledger reconciliation across all test scenarios including failure and retry paths.
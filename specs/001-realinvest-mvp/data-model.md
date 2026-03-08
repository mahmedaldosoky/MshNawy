# Data Model: MshNawy (مش ناوي) MVP

**Branch**: `001-realinvest-mvp` | **Date**: 2026-02-28 | **Updated**: 2026-03-08

> All monetary amounts stored as `long` (piasters, 1 EGP = 100 piasters).
> All entities use ABP's `FullAuditedAggregateRoot` or `FullAuditedEntity` as appropriate.
> Timestamps in UTC. Optimistic concurrency via `ConcurrencyStamp` on all aggregate roots.

## Entity Relationship Diagram (Text)

```
User 1──1 Wallet
User 1──* Deposit
User 1──* Withdrawal
User 1──* InvestmentOrder
User 1──* Holding
User 1──* SaleVote
User 1──* SupportTicket
User 1──* Notification
User 1──0..1 KnowledgeCheckRecord

Wallet 1──* LedgerEntry (via account identifiers)

Offering 1──* InvestmentOrder
Offering 1──1 OfferingFinancialModel
Offering *──1 FeePolicy (via snapshot at order time)

InvestmentOrder 1──* Installment
InvestmentOrder 1──1 Holding (on settlement)
InvestmentOrder *──* LedgerEntry (via reference)

Offering 1──0..1 PropertySale
PropertySale 1──* SaleDistribution
SaleVote *──1 Offering (unique per user+offering)

SupportTicket 1──* TicketMessage
SupportTicket 1──* TicketAttachment
```

## Entities

### User (Aggregate Root)

Extends ABP's `IdentityUser`. Additional fields:

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| PhoneNumber | string(15) | Required, unique, format: +20XXXXXXXXXX | Primary login identifier |
| KycStatus | KycStatus (enum) | Required | Draft, Submitted, UnderReview, Approved, Rejected, NeedsResubmission |
| KycRejectionReason | string(500) | Nullable | Arabic text, set on rejection |
| FullNameArabic | string(200) | Nullable | Required for KYC submission |
| DateOfBirth | DateTime | Nullable | Required for KYC |
| NationalIdNumber | string(14) | Nullable, unique when set | Egyptian national ID |
| NationalIdFrontImagePath | string(500) | Nullable | Secure file path |
| NationalIdBackImagePath | string(500) | Nullable | Secure file path |
| SelfiePath | string(500) | Nullable | Secure file path |
| OtpAttemptCount | int | Default 0 | Reset every 15-min window |
| OtpWindowStart | DateTime | Nullable | Start of current 15-min window |
| OtpLockedUntil | DateTime | Nullable | Set when 5 attempts exceeded |

**State machine (KYC)**:
```
Draft → Submitted (user submits documents)
Submitted → UnderReview (admin picks up)
UnderReview → Approved (admin approves)
UnderReview → Rejected (admin rejects with reason)
UnderReview → NeedsResubmission (admin requests fixes)
Rejected → Submitted (user resubmits — clears rejection reason)
NeedsResubmission → Submitted (user resubmits)
```

### LedgerEntry (Entity — immutable, NOT an aggregate root)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| Timestamp | DateTime | Required, UTC | |
| EntryType | LedgerEntryType (enum) | Required | Deposit, Withdrawal, OrderReservation, OrderSettlement, OrderFailure, InstallmentPayment, ExitSettlement, FeeCollection, Compensating |
| DebitAccount | string(100) | Required | Account identifier (e.g., "User:Available:{userId}") |
| CreditAccount | string(100) | Required | Account identifier |
| Amount | long | Required, > 0 | In piasters |
| IdempotencyKey | Guid | Required, unique | Client-generated |
| ReferenceEntityType | string(50) | Required | "Deposit", "Withdrawal", "Order", "PropertySale", "Installment" |
| ReferenceEntityId | Guid | Required | FK to the source entity |
| Description | string(500) | Required | Human-readable Arabic description |
| CompensatingEntryId | Guid | Nullable | Points to original entry if this is a reversal |
| PostedByUserId | Guid | Nullable | Admin or system user who posted |

**Indexes**: Unique on `IdempotencyKey`. Index on `DebitAccount`, `CreditAccount`, `ReferenceEntityId`.

### Deposit (Aggregate Root)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| UserId | Guid | Required, FK | |
| Method | DepositMethod (enum) | Required | InstaPay, VodafoneCash, BankTransfer |
| Amount | long | Required, > 0 | In piasters |
| ReferenceCode | string(20) | Required, unique | System-generated |
| ProofImagePath | string(500) | Nullable | Uploaded after external transfer |
| Status | DepositStatus (enum) | Required | Created, PendingReview, Approved, Rejected, Posted |
| ReviewerNotes | string(500) | Nullable | Admin notes (Arabic) |
| ReviewedByUserId | Guid | Nullable | Admin who reviewed |
| ReviewedAt | DateTime | Nullable | |

**State machine**:
```
Created → PendingReview (user uploads proof)
PendingReview → Approved (admin approves)
PendingReview → Rejected (admin rejects with reason)
Approved → Posted (ledger entries created, balance updated)
```

### Withdrawal (Aggregate Root)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| UserId | Guid | Required, FK | |
| Method | WithdrawalMethod (enum) | Required | BankTransfer, VodafoneCash |
| Amount | long | Required, > 0 | In piasters (requested amount) |
| Fee | long | Required, >= 0 | 0 for bank transfer, 500 piasters for Vodafone Cash |
| NetAmount | long | Required, > 0 | Amount - Fee (what user receives) |
| IdempotencyKey | Guid | Required, unique | Client-generated |
| DestinationBankName | string(100) | Nullable | For bank transfer |
| DestinationAccountHolder | string(200) | Nullable | For bank transfer |
| DestinationAccountNumber | string(34) | Nullable | Account number or IBAN |
| DestinationVodafoneNumber | string(15) | Nullable | For Vodafone Cash |
| Status | WithdrawalStatus (enum) | Required | Created, PendingReview, Processing, Completed, Rejected |
| ReviewerNotes | string(500) | Nullable | |
| ReviewedByUserId | Guid | Nullable | |
| ReviewedAt | DateTime | Nullable | |

**State machine**:
```
Created → PendingReview (user confirms, funds reserved from Available)
PendingReview → Processing (admin approves, initiates external transfer)
Processing → Completed (external transfer confirmed)
PendingReview → Rejected (admin rejects, compensating entries return funds)
Processing → Rejected (transfer failed, compensating entries return funds)
```

### Offering (Aggregate Root)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| Name | string(200) | Required | Arabic property name |
| Location | string(300) | Required | Arabic address/area |
| Description | string(2000) | Required | Arabic description |
| PropertyPrice | long | Required, > 0 | Total property value in piasters |
| TotalShares | int | Required, > 0 | |
| AvailableShares | int | Required, >= 0 | Decremented on settlement |
| SharePrice | long | Required, > 0 | PropertyPrice / TotalShares (in piasters) |
| RiskLevel | RiskLevel (enum) | Required | Low, Medium, High |
| Status | OfferingStatus (enum) | Required | Draft, Open, Closed, SaleInitiated, Settled |
| MaturityDate | DateTime | Required | When the offering matures (triggers admin notification) |
| SaleVoteThresholdPercent | decimal(5,2) | Required, default 50.01 | % of shares needed to trigger sale vote |
| FeePolicyId | Guid | Required, FK | Active fee policy at offering creation |
| DisclaimerText | string(1000) | Required | Arabic disclaimer about projections |

**Images**: Separate `OfferingImage` entity (OfferingId, ImagePath, SortOrder).

**State machine**:
```
Draft → Open (admin publishes)
Open → Closed (admin closes or fully subscribed)
Closed → SaleInitiated (majority vote reached, maturity, or platform decision)
SaleInitiated → Settled (property sold, proceeds distributed)
SaleInitiated → Closed (sale cancelled, offering returns to closed)
```

### OfferingFinancialModel (Entity, owned by Offering)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| OfferingId | Guid | Required, FK, unique | 1:1 with Offering |
| DownPaymentPercent | decimal(5,2) | Required | e.g., 25.00 for 25% |
| InstallmentCount | int | Required, >= 0 | 0 means full upfront payment |
| InstallmentStartDate | DateTime | Nullable | First installment due date |
| GracePeriodDays | int | Required, default 7 | |
| ExpectedMonthlyRent | long | Required | In piasters |
| OccupancyRateBase | decimal(5,2) | Required | e.g., 85.00 for 85% |
| OccupancyRateConservative | decimal(5,2) | Required | |
| OccupancyRateOptimistic | decimal(5,2) | Required | |
| AppreciationRateBase | decimal(5,2) | Required | Annual % |
| AppreciationRateConservative | decimal(5,2) | Required | |
| AppreciationRateOptimistic | decimal(5,2) | Required | |
| RentGrowthRateBase | decimal(5,2) | Required | Annual % |
| RentGrowthRateConservative | decimal(5,2) | Required | |
| RentGrowthRateOptimistic | decimal(5,2) | Required | |
| ProjectedExitDate | DateTime | Required | Used for projection calculations (may differ from Offering.MaturityDate) |

### FeePolicy (Aggregate Root)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| Version | int | Required, unique per date range | Auto-incremented |
| EffectiveFrom | DateTime | Required | |
| EffectiveTo | DateTime | Nullable | Null = current policy |
| EntryFeePercent | decimal(5,2) | Required | Default 1.00 |
| PaymentFeePercent | decimal(5,2) | Required | Default 3.00 |
| ExitFeePercent | decimal(5,2) | Required | Default 5.00 |
| ExitBrokeragePercent | decimal(5,2) | Required | Default 2.50 |
| ExitPlatformProfitPercent | decimal(5,2) | Required | Default 2.50 |
| CreatedByUserId | Guid | Required | Admin who created |

**Constraint**: ExitBrokeragePercent + ExitPlatformProfitPercent = ExitFeePercent (enforced in domain).

### InvestmentOrder (Aggregate Root)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| UserId | Guid | Required, FK | |
| OfferingId | Guid | Required, FK | |
| ShareCount | int | Required, > 0 | |
| SharePrice | long | Required | Snapshot at order time (piasters) |
| TotalShareCost | long | Required | ShareCount * SharePrice |
| EntryFee | long | Required | Calculated from fee policy |
| DownPaymentAmount | long | Required | Based on payment plan % |
| TotalCost | long | Required | Full cost including all fees |
| IdempotencyKey | Guid | Required, unique | |
| FeePolicyId | Guid | Required, FK | Snapshot of fee policy at order time |
| Status | OrderStatus (enum) | Required | Created, Reserved, Submitted, Settled, Failed |
| FailureReason | string(500) | Nullable | Arabic |
| SettledByUserId | Guid | Nullable | Admin |
| SettledAt | DateTime | Nullable | |

**State machine**:
```
Created → Reserved (funds moved Available→Reserved, atomic)
Reserved → Submitted (awaiting admin settlement)
Submitted → Settled (admin approves, Reserved→Invested, shares issued)
Submitted → Failed (settlement fails, compensating entries Reserved→Available)
Reserved → Failed (timeout or error, compensating entries)
```

### Installment (Entity, owned by InvestmentOrder)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| InvestmentOrderId | Guid | Required, FK | |
| InstallmentNumber | int | Required | 1-based sequential |
| DueDate | DateTime | Required | |
| Amount | long | Required | In piasters |
| FeeAmount | long | Required | 3% payment fee in piasters |
| TotalDue | long | Required | Amount + FeeAmount |
| Status | InstallmentStatus (enum) | Required | Upcoming, Due, Paid, Overdue, GracePeriod, Flagged |
| PaidAt | DateTime | Nullable | |
| GracePeriodEndsAt | DateTime | Nullable | DueDate + GracePeriodDays |
| LedgerEntryId | Guid | Nullable | FK to payment ledger entry |

**State machine**:
```
Upcoming → Due (due date reached by scheduler)
Due → Paid (sufficient Available balance, auto-deducted)
Due → Overdue (insufficient balance on due date)
Overdue → GracePeriod (grace period timer starts)
GracePeriod → Paid (user deposits and balance covers it)
GracePeriod → Flagged (grace period expired, back-office review)
Flagged → Paid (admin resolves)
Upcoming → Cancelled (property sale settled, remaining installments cancelled)
Due → Cancelled (property sale settled)
Overdue → Cancelled (property sale settled)
GracePeriod → Cancelled (property sale settled)
```

### Holding (Aggregate Root)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| UserId | Guid | Required, FK | |
| OfferingId | Guid | Required, FK | |
| SharesOwned | int | Required, > 0 | |
| CostBasis | long | Required | Total paid including fees (piasters) |
| FeesPaid | long | Required | Accumulated fees (piasters) |
| AcquisitionDate | DateTime | Required | |
| InvestmentOrderId | Guid | Required, FK | |
| IsActive | bool | Required, default true | False after exit completed |

### SaleVote (Entity)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| UserId | Guid | Required, FK | |
| OfferingId | Guid | Required, FK | |
| VoteDirection | SaleVoteDirection (enum) | Required | Sell, Hold |
| VotedAt | DateTime | Required | |

**Constraint**: Unique on (UserId, OfferingId). Users can change their vote until a PropertySale is initiated.

### PropertySale (Aggregate Root)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| OfferingId | Guid | Required, FK, unique | One active sale per offering |
| TriggerType | SaleTriggerType (enum) | Required | MajorityVote, Maturity, PlatformDecision |
| ActualSalePrice | long | Nullable | Set by admin when property sold (piasters) |
| TotalExitFee | long | Nullable | 5% of ActualSalePrice |
| TotalBrokerageFee | long | Nullable | 2.5% of ActualSalePrice |
| TotalPlatformFee | long | Nullable | 2.5% of ActualSalePrice |
| TotalNetProceeds | long | Nullable | ActualSalePrice - TotalExitFee |
| Status | PropertySaleStatus (enum) | Required | Initiated, Listed, Sold, Distributing, Settled, Cancelled |
| InitiatedByUserId | Guid | Nullable | Admin who initiated |
| InitiatedAt | DateTime | Required | |
| SettledAt | DateTime | Nullable | |

**State machine**:
```
Initiated → Listed (admin lists property on market)
Listed → Sold (admin records actual sale price)
Sold → Distributing (admin triggers distribution calculation)
Distributing → Settled (all distributions posted to holders' Available balances via ledger)
Listed → Cancelled (sale falls through, offering reopens)
Initiated → Cancelled (admin cancels before listing)
```

### SaleDistribution (Entity, owned by PropertySale)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| PropertySaleId | Guid | Required, FK | |
| UserId | Guid | Required, FK | |
| HoldingId | Guid | Required, FK | |
| SharesHeld | int | Required | Snapshot at distribution time |
| GrossProceeds | long | Required | (SharesHeld / TotalShares) * ActualSalePrice (piasters) |
| ExitFee | long | Required | 5% of GrossProceeds |
| NetProceeds | long | Required | GrossProceeds - ExitFee |
| LedgerEntryId | Guid | Nullable | FK to ledger entry when posted |
| DistributedAt | DateTime | Nullable | |

### SupportTicket (Aggregate Root)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| UserId | Guid | Required, FK | |
| ReferenceNumber | string(20) | Required, unique | System-generated |
| Subject | string(200) | Required | Arabic |
| Category | TicketCategory (enum) | Required | General, Technical, Financial, KYC, Complaint |
| Description | string(2000) | Required | Arabic |
| Status | TicketStatus (enum) | Required | Open, InProgress, Resolved, Closed |
| SlaDeadline | DateTime | Required | CreatedAt + 48 hours |
| IsSlaBreach | bool | Required, default false | |

### TicketMessage (Entity, owned by SupportTicket)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| SupportTicketId | Guid | Required, FK | |
| SenderUserId | Guid | Required | User or admin |
| IsAdminReply | bool | Required | |
| Content | string(2000) | Required | Arabic |
| SentAt | DateTime | Required | |

### TicketAttachment (Entity, owned by SupportTicket)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| SupportTicketId | Guid | Required, FK | |
| TicketMessageId | Guid | Nullable, FK | Null = attached to initial ticket |
| FilePath | string(500) | Required | |
| FileName | string(200) | Required | Original filename |
| FileSize | long | Required | In bytes, max 5 MB |
| ContentType | string(50) | Required | image/jpeg, image/png |

### KnowledgeCheckRecord (Entity)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| UserId | Guid | Required, FK | |
| RiskLevelAcknowledged | RiskLevel (enum) | Required | Highest risk level passed |
| CompletedAt | DateTime | Required | |

### Notification (Entity)

| Field | Type | Constraints | Notes |
|-------|------|-------------|-------|
| Id | Guid | PK | |
| UserId | Guid | Required, FK | |
| EventType | NotificationEventType (enum) | Required | KycStatusChange, DepositPosted, DepositRejected, WithdrawalCompleted, WithdrawalRejected, OrderSettled, OrderFailed, InstallmentDue, InstallmentOverdue, SaleVoteThresholdReached, PropertySaleUpdate, SaleProceedsDistributed, TicketResponse |
| ReferenceEntityType | string(50) | Required | |
| ReferenceEntityId | Guid | Required | |
| MessageArabic | string(500) | Required | |
| LinkTarget | string(200) | Required | Route path (e.g., "/wallet/deposits/{id}") |
| IsRead | bool | Required, default false | |
| CreatedAt | DateTime | Required | |

**Index**: UserId + IsRead + CreatedAt (DESC) for efficient unread badge queries.

## Enumerations Summary

| Enum | Values |
|------|--------|
| KycStatus | Draft, Submitted, UnderReview, Approved, Rejected, NeedsResubmission |
| DepositMethod | InstaPay, VodafoneCash, BankTransfer |
| DepositStatus | Created, PendingReview, Approved, Rejected, Posted |
| WithdrawalMethod | BankTransfer, VodafoneCash |
| WithdrawalStatus | Created, PendingReview, Processing, Completed, Rejected |
| RiskLevel | Low, Medium, High |
| OfferingStatus | Draft, Open, Closed, SaleInitiated, Settled |
| OrderStatus | Created, Reserved, Submitted, Settled, Failed |
| InstallmentStatus | Upcoming, Due, Paid, Overdue, GracePeriod, Flagged, Cancelled |
| SaleVoteDirection | Sell, Hold |
| SaleTriggerType | MajorityVote, Maturity, PlatformDecision |
| PropertySaleStatus | Initiated, Listed, Sold, Distributing, Settled, Cancelled |
| TicketCategory | General, Technical, Financial, KYC, Complaint |
| TicketStatus | Open, InProgress, Resolved, Closed |
| LedgerEntryType | Deposit, Withdrawal, OrderReservation, OrderSettlement, OrderFailure, InstallmentPayment, SaleProceeds, SaleExitFee, FeeCollection, Compensating |
| NotificationEventType | KycStatusChange, DepositPosted, DepositRejected, WithdrawalCompleted, WithdrawalRejected, OrderSettled, OrderFailed, InstallmentDue, InstallmentOverdue, SaleVoteThresholdReached, PropertySaleUpdate, SaleProceedsDistributed, TicketResponse |

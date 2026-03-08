# API Contracts: MshNawy (مش ناوي) MVP

**Branch**: `001-realinvest-mvp` | **Date**: 2026-02-28 | **Updated**: 2026-03-08

> All endpoints follow ABP conventions: `/api/app/{resource}`.
> All monetary values in responses are in piasters (long). Frontend converts to EGP for display.
> All endpoints require authentication unless marked `[Anonymous]`.
> Admin endpoints require `MshNawy.Admin.*` permissions.

## Base URL

- Investor API: `/api/app`
- Admin API: `/api/app/admin`

## Common Patterns

**Error Response** (ABP standard):
```json
{
  "error": {
    "code": "MshNawy:ErrorCode",
    "message": "Arabic error message",
    "details": "Optional details",
    "validationErrors": [
      { "message": "Field error", "members": ["fieldName"] }
    ]
  }
}
```

**Pagination** (ABP standard):
```json
{
  "items": [...],
  "totalCount": 100
}
```

**Idempotency**: All mutating financial endpoints accept `X-Idempotency-Key` header (UUID).

---

## 1. Authentication & OTP

### POST /api/app/auth/send-otp `[Anonymous]`

Request:
```json
{ "phoneNumber": "+201234567890" }
```
Response (200):
```json
{ "expiresInSeconds": 180, "attemptsRemaining": 4 }
```
Errors: `RealInvest:PhoneLocked` (429), `RealInvest:InvalidPhoneFormat` (400)

### POST /api/app/auth/verify-otp `[Anonymous]`

Request:
```json
{ "phoneNumber": "+201234567890", "otpCode": "123456" }
```
Response (200):
```json
{ "accessToken": "jwt...", "refreshToken": "...", "userId": "guid", "kycStatus": "Draft" }
```
Errors: `RealInvest:InvalidOtp` (400), `RealInvest:OtpExpired` (400), `RealInvest:PhoneLocked` (429)

---

## 2. KYC

### GET /api/app/kyc/status

Response (200):
```json
{
  "status": "Draft|Submitted|UnderReview|Approved|Rejected|NeedsResubmission",
  "rejectionReason": "Arabic reason or null",
  "submittedAt": "2026-02-28T10:00:00Z"
}
```

### POST /api/app/kyc/submit

Request (multipart/form-data):
```
fullNameArabic: string
dateOfBirth: date
nationalIdNumber: string (14 digits)
nationalIdFront: file (JPEG/PNG, max 5MB)
nationalIdBack: file (JPEG/PNG, max 5MB)
selfie: file (JPEG/PNG, max 5MB)
```
Response (200):
```json
{ "status": "Submitted", "submittedAt": "2026-02-28T10:00:00Z" }
```
Errors: `RealInvest:KycAlreadyApproved` (400), `RealInvest:InvalidNationalId` (400), `RealInvest:FileTooLarge` (400)

### Admin: GET /api/app/admin/kyc?status=Submitted&skipCount=0&maxResultCount=10

Response: Paginated list of KYC submissions with user details and document URLs.

### Admin: POST /api/app/admin/kyc/{userId}/review

Request:
```json
{ "decision": "Approve|Reject|NeedsResubmission", "reason": "Arabic reason (required for Reject/NeedsResubmission)" }
```

---

## 3. Wallet

### GET /api/app/wallet

Response (200):
```json
{
  "available": 500000,
  "reserved": 100000,
  "invested": 2000000,
  "pendingWithdrawal": 0,
  "currency": "EGP"
}
```

### GET /api/app/wallet/transactions?skipCount=0&maxResultCount=20&type=All

Response (200):
```json
{
  "items": [
    {
      "id": "guid",
      "type": "Deposit|Withdrawal|Investment|Exit|InstallmentPayment|Fee",
      "amount": 100000,
      "description": "Arabic description",
      "status": "Completed|Pending|Failed",
      "createdAt": "2026-02-28T10:00:00Z",
      "referenceEntityType": "Deposit",
      "referenceEntityId": "guid"
    }
  ],
  "totalCount": 50
}
```

---

## 4. Deposits

### GET /api/app/deposits/payment-details/{method}

Response (200):
```json
{
  "method": "InstaPay",
  "accountDetails": {
    "ipaAddress": "realinvest@instapay",
    "bankName": null,
    "accountNumber": null,
    "iban": null,
    "vodafoneNumber": null
  },
  "referenceCode": "DEP-2026-XXXXXX"
}
```

### POST /api/app/deposits

Request (multipart/form-data):
```
method: "InstaPay"|"VodafoneCash"|"BankTransfer"
amount: long (piasters)
referenceCode: string
proofImage: file (JPEG/PNG, max 5MB)
```
Headers: `X-Idempotency-Key: uuid`

Response (201):
```json
{
  "id": "guid",
  "referenceCode": "DEP-2026-XXXXXX",
  "amount": 100000,
  "method": "InstaPay",
  "status": "PendingReview",
  "createdAt": "2026-02-28T10:00:00Z"
}
```

### GET /api/app/deposits?skipCount=0&maxResultCount=10

Response: Paginated list of user's deposits with status.

### Admin: POST /api/app/admin/deposits/{id}/review

Request:
```json
{ "decision": "Approve|Reject", "notes": "Arabic notes" }
```

---

## 5. Withdrawals

### POST /api/app/withdrawals

Request:
```json
{
  "method": "BankTransfer|VodafoneCash",
  "amount": 100000,
  "bankName": "بنك مصر",
  "accountHolderName": "اسم المستخدم",
  "accountNumber": "1234567890",
  "vodafoneNumber": null
}
```
Headers: `X-Idempotency-Key: uuid`

Response (201):
```json
{
  "id": "guid",
  "amount": 100000,
  "fee": 0,
  "netAmount": 100000,
  "method": "BankTransfer",
  "status": "PendingReview",
  "createdAt": "2026-02-28T10:00:00Z"
}
```
Errors: `RealInvest:InsufficientBalance` (400), `RealInvest:InsufficientBalanceForFee` (400)

### GET /api/app/withdrawals?skipCount=0&maxResultCount=10

Response: Paginated list of user's withdrawals.

### Admin: POST /api/app/admin/withdrawals/{id}/review

Request:
```json
{ "decision": "Approve|Reject", "notes": "Arabic notes" }
```

---

## 6. Offerings

### GET /api/app/offerings?skipCount=0&maxResultCount=10&status=Open

Response (200):
```json
{
  "items": [
    {
      "id": "guid",
      "name": "Arabic property name",
      "location": "Arabic location",
      "sharePrice": 50000,
      "totalShares": 1000,
      "availableShares": 750,
      "riskLevel": "Medium",
      "projectedAnnualReturnRange": { "conservative": 800, "base": 1200, "optimistic": 1800 },
      "briefDescription": "Arabic brief",
      "imageUrl": "/api/app/offerings/guid/images/0"
    }
  ],
  "totalCount": 5
}
```

### GET /api/app/offerings/{id}

Response (200):
```json
{
  "id": "guid",
  "name": "Arabic property name",
  "location": "Arabic location",
  "description": "Arabic full description",
  "propertyPrice": 50000000,
  "sharePrice": 50000,
  "totalShares": 1000,
  "availableShares": 750,
  "riskLevel": "Medium",
  "status": "Open",
  "disclaimer": "Arabic disclaimer text",
  "images": ["/api/app/offerings/guid/images/0", "..."],
  "paymentPlan": {
    "downPaymentPercent": 25.00,
    "installmentCount": 12,
    "installmentStartDate": "2026-04-01",
    "gracePeriodDays": 7
  },
  "feeBreakdown": {
    "entryFeePercent": 1.00,
    "paymentFeePercent": 3.00,
    "exitFeePercent": 5.00,
    "exitBrokeragePercent": 2.50,
    "exitPlatformProfitPercent": 2.50
  }
}
```

### GET /api/app/offerings/{id}/projections?scenario=Base

Response (200):
```json
{
  "scenario": "Base",
  "inputs": {
    "occupancyRate": 85.00,
    "appreciationRate": 7.00,
    "rentGrowthRate": 5.00,
    "expectedMonthlyRent": 5000000,
    "exitDate": "2031-02-28"
  },
  "outputs": {
    "projectedAnnualReturnPercent": 12.50,
    "projectedDistributionSchedule": [
      { "date": "2026-06-01", "amount": 425000, "label": "Arabic label" }
    ],
    "projectedExitValue": 60000000,
    "totalFeeImpact": 2500000,
    "netReturnAfterFees": 7500000,
    "paymentTimeline": [
      { "date": "2026-03-01", "type": "DownPayment", "amount": 12500, "fee": 500 },
      { "date": "2026-04-01", "type": "Installment", "number": 1, "amount": 3125, "fee": 94 }
    ]
  }
}
```

### Admin: POST /api/app/admin/offerings

Request: Full offering creation with financial model inputs.

### Admin: PUT /api/app/admin/offerings/{id}

Request: Update offering details and model inputs.

### Admin: POST /api/app/admin/offerings/{id}/status

Request:
```json
{ "status": "Open|Closed" }
```

---

## 7. Investment Orders

### GET /api/app/knowledge-check/status

Response (200):
```json
{ "highestRiskLevelPassed": "Medium|null", "completedAt": "2026-02-28T10:00:00Z|null" }
```

### POST /api/app/knowledge-check/submit

Request:
```json
{ "riskLevel": "Low|Medium|High", "answers": [{ "questionId": 1, "answerId": 2 }] }
```
Response (200):
```json
{ "passed": true, "riskLevelAcknowledged": "Medium" }
```

### POST /api/app/orders

Request:
```json
{ "offeringId": "guid", "shareCount": 5 }
```
Headers: `X-Idempotency-Key: uuid`

Response (201):
```json
{
  "id": "guid",
  "offeringId": "guid",
  "shareCount": 5,
  "sharePrice": 50000,
  "totalShareCost": 250000,
  "entryFee": 2500,
  "downPaymentAmount": 63125,
  "totalCost": 257500,
  "status": "Reserved",
  "paymentSchedule": [
    { "installmentNumber": 0, "type": "DownPayment", "dueDate": "2026-03-01", "amount": 63125, "fee": 2500 },
    { "installmentNumber": 1, "type": "Installment", "dueDate": "2026-04-01", "amount": 15625, "fee": 469 }
  ]
}
```
Errors: `RealInvest:InsufficientBalance` (400), `RealInvest:OfferingClosed` (400), `RealInvest:InsufficientShares` (400), `RealInvest:KnowledgeCheckRequired` (400)

### GET /api/app/orders?skipCount=0&maxResultCount=10

Response: Paginated list of user's orders.

### Admin: POST /api/app/admin/orders/{id}/settle

Request:
```json
{ "decision": "Settle|Fail", "reason": "Arabic reason (required for Fail)" }
```

---

## 8. Portfolio & Holdings

### GET /api/app/portfolio/summary

Response (200):
```json
{
  "totalInvested": 2000000,
  "totalProjectedValueRange": { "conservative": 2200000, "base": 2600000, "optimistic": 3100000 },
  "totalFeesPaid": 50000,
  "activeHoldingsCount": 3
}
```

### GET /api/app/portfolio/holdings?skipCount=0&maxResultCount=10

Response (200):
```json
{
  "items": [
    {
      "id": "guid",
      "offeringName": "Arabic name",
      "sharesOwned": 5,
      "costBasis": 260000,
      "paidToDate": 130000,
      "feesPaidToDate": 5000,
      "projectedValueRange": { "conservative": 280000, "base": 320000, "optimistic": 380000 },
      "acquisitionDate": "2026-03-01",
      "isActive": true
    }
  ],
  "totalCount": 3
}
```

### GET /api/app/portfolio/holdings/{id}/activity?skipCount=0&maxResultCount=20

Response: Paginated list of holding activity (purchases, payments, distributions).

### GET /api/app/portfolio/statement?from=2026-01-01&to=2026-12-31

Response: HTML document (Content-Type: text/html; charset=utf-8). Arabic-formatted statement.

---

## 9. Sale Votes & Property Sales

### GET /api/app/offerings/{offeringId}/sale-vote

Response (200):
```json
{
  "offeringId": "guid",
  "totalShares": 1000,
  "sellVoteShares": 350,
  "sellVotePercent": 35.00,
  "threshold": 50.01,
  "userVote": "Sell|Hold|null",
  "maturityDate": "2031-02-28",
  "saleStatus": "None|Initiated|Listed|Sold|Distributing|Settled"
}
```

### POST /api/app/offerings/{offeringId}/sale-vote

Request:
```json
{ "direction": "Sell|Hold" }
```
Response (200):
```json
{
  "userVote": "Sell",
  "sellVotePercent": 55.50,
  "thresholdReached": true
}
```
Errors: `RealInvest:NotAHolder` (403), `RealInvest:SaleAlreadyInitiated` (400)

### DELETE /api/app/offerings/{offeringId}/sale-vote

Withdraws the user's vote. Only allowed before a PropertySale is initiated.

### GET /api/app/property-sales/{offeringId}

Response (200):
```json
{
  "id": "guid",
  "offeringId": "guid",
  "triggerType": "MajorityVote|Maturity|PlatformDecision",
  "status": "Initiated|Listed|Sold|Distributing|Settled|Cancelled",
  "actualSalePrice": null,
  "userDistribution": {
    "sharesHeld": 5,
    "grossProceeds": null,
    "exitFee": null,
    "netProceeds": null,
    "distributedAt": null
  },
  "exitFeeBreakdown": {
    "totalPercent": 5.00,
    "brokeragePercent": 2.50,
    "platformProfitPercent": 2.50
  },
  "initiatedAt": "2026-02-28T10:00:00Z"
}
```
Errors: `RealInvest:NoActiveSale` (404)

### Admin: POST /api/app/admin/property-sales/initiate

Request:
```json
{ "offeringId": "guid", "triggerType": "MajorityVote|Maturity|PlatformDecision" }
```

### Admin: POST /api/app/admin/property-sales/{id}/update-status

Request:
```json
{
  "status": "Listed|Sold|Distributing|Cancelled",
  "actualSalePrice": 55000000,
  "notes": "Arabic notes"
}
```

### Admin: POST /api/app/admin/property-sales/{id}/settle

Triggers distribution: calculates pro-rata proceeds for all holders, posts ledger entries, deactivates holdings, cancels remaining installments, settles the offering.

Response (200):
```json
{
  "distributionCount": 150,
  "totalDistributed": 52250000,
  "totalExitFees": 2750000
}
```

---

## 10. Support Tickets

### POST /api/app/support/tickets

Request (multipart/form-data):
```
subject: string
category: "General"|"Technical"|"Financial"|"KYC"|"Complaint"
description: string
attachments: file[] (max 3, JPEG/PNG, max 5MB each)
```

Response (201):
```json
{
  "id": "guid",
  "referenceNumber": "TKT-2026-XXXXXX",
  "status": "Open",
  "slaDeadline": "2026-03-02T10:00:00Z",
  "createdAt": "2026-02-28T10:00:00Z"
}
```

### GET /api/app/support/tickets?skipCount=0&maxResultCount=10

Response: Paginated list of user's tickets.

### GET /api/app/support/tickets/{id}

Response: Full ticket with message thread.

### POST /api/app/support/tickets/{id}/messages

Request (multipart/form-data):
```
content: string
attachments: file[] (max 3)
```

### Admin: GET /api/app/admin/support/tickets?status=Open&skipCount=0&maxResultCount=10

### Admin: POST /api/app/admin/support/tickets/{id}/messages

### Admin: POST /api/app/admin/support/tickets/{id}/status

Request:
```json
{ "status": "InProgress|Resolved|Closed" }
```

---

## 11. Notifications

### GET /api/app/notifications?skipCount=0&maxResultCount=20&isRead=false

Response (200):
```json
{
  "items": [
    {
      "id": "guid",
      "eventType": "DepositPosted",
      "messageArabic": "تم إيداع مبلغ ١٠٠٠ ج.م. في محفظتك",
      "linkTarget": "/wallet/deposits/guid",
      "isRead": false,
      "createdAt": "2026-02-28T10:00:00Z"
    }
  ],
  "totalCount": 15,
  "unreadCount": 5
}
```

### POST /api/app/notifications/{id}/read

Response (204): No content.

### POST /api/app/notifications/read-all

Response (204): No content.

---

## 12. Admin: Fee Policy

### GET /api/app/admin/fee-policies?skipCount=0&maxResultCount=10

Response: Paginated list of fee policies with version history.

### POST /api/app/admin/fee-policies

Request:
```json
{
  "effectiveFrom": "2026-04-01",
  "entryFeePercent": 1.00,
  "paymentFeePercent": 3.00,
  "exitFeePercent": 5.00,
  "exitBrokeragePercent": 2.50,
  "exitPlatformProfitPercent": 2.50
}
```

## Error Codes

| Code | HTTP | Description |
|------|------|-------------|
| RealInvest:InvalidPhoneFormat | 400 | Phone number not +20XXXXXXXXXX format |
| RealInvest:PhoneLocked | 429 | OTP rate limit exceeded |
| RealInvest:InvalidOtp | 400 | Wrong OTP code |
| RealInvest:OtpExpired | 400 | OTP expired (>3 min) |
| RealInvest:KycAlreadyApproved | 400 | KYC already approved |
| RealInvest:KycRequired | 403 | Operation requires approved KYC |
| RealInvest:InvalidNationalId | 400 | National ID not 14 digits |
| RealInvest:FileTooLarge | 400 | File exceeds 5 MB |
| RealInvest:InsufficientBalance | 400 | Not enough available balance |
| RealInvest:InsufficientBalanceForFee | 400 | Balance doesn't cover amount + fee |
| RealInvest:OfferingClosed | 400 | Offering not open for investment |
| RealInvest:InsufficientShares | 400 | Not enough available shares |
| RealInvest:KnowledgeCheckRequired | 400 | Must complete knowledge check first |
| RealInvest:DuplicateRequest | 409 | Idempotency key already processed |
| RealInvest:InvalidStateTransition | 400 | Entity not in valid state for operation |
| RealInvest:NotAHolder | 403 | User has no shares in this offering |
| RealInvest:SaleAlreadyInitiated | 400 | Property sale already in progress |
| RealInvest:NoActiveSale | 404 | No active property sale for this offering |

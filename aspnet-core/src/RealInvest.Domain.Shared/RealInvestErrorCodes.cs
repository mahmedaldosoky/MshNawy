namespace RealInvest.Domain.Shared
{
    /// <summary>
    /// RealInvest error codes for API responses and domain exceptions.
    /// Format: "RealInvest:NNNN"
    /// </summary>
    public static class RealInvestErrorCodes
    {
        // === Generic (0001–0099) ===
        public const string DefaultError = "RealInvest:0001";
        public const string InvalidInput = "RealInvest:0002";
        public const string UnexpectedError = "RealInvest:0003";
        public const string Unauthorized = "RealInvest:0004";
        public const string Forbidden = "RealInvest:0005";
        public const string NotFound = "RealInvest:0006";
        public const string ValidationFailed = "RealInvest:0007";
        public const string ConcurrencyFailed = "RealInvest:0008";

        // === OTP & Authentication (0100–0199) ===
        public const string InvalidPhoneFormat = "RealInvest:0100";
        public const string OtpRequestFailed = "RealInvest:0101";
        public const string OtpInvalid = "RealInvest:0102";
        public const string OtpExpired = "RealInvest:0103";
        public const string OtpRateLimited = "RealInvest:0104";
        public const string OtpPhoneLockedOut = "RealInvest:0105";
        public const string InvalidCredentials = "RealInvest:0106";
        public const string SessionExpired = "RealInvest:0107";

        // === KYC Verification (0200–0299) ===
        public const string KycNotStarted = "RealInvest:0200";
        public const string KycNotApproved = "RealInvest:0201";
        public const string KycPending = "RealInvest:0202";
        public const string KycRejected = "RealInvest:0203";
        public const string KycNeedsResubmission = "RealInvest:0204";
        public const string KycInvalidNationalId = "RealInvest:0205";
        public const string KycImageUploadFailed = "RealInvest:0206";
        public const string KycImageInvalid = "RealInvest:0207";
        public const string KycUnderReview = "RealInvest:0208";

        // === Wallet & Balance (0300–0399) ===
        public const string InsufficientBalance = "RealInvest:0300";
        public const string InsufficientAvailableBalance = "RealInvest:0301";
        public const string InvalidAmount = "RealInvest:0302";
        public const string NegativeAmount = "RealInvest:0303";
        public const string AmountExceedsMaximum = "RealInvest:0304";
        public const string WalletNotFound = "RealInvest:0305";

        // === Deposits (0400–0499) ===
        public const string InvalidDepositMethod = "RealInvest:0400";
        public const string DepositAlreadyProcessed = "RealInvest:0401";
        public const string DepositProofInvalid = "RealInvest:0402";
        public const string DepositProofMissing = "RealInvest:0403";
        public const string DepositReferenceMissing = "RealInvest:0404";
        public const string DepositNotFound = "RealInvest:0405";
        public const string DepositCannotBeModified = "RealInvest:0406";
        public const string DepositAmountMismatch = "RealInvest:0407";

        // === Withdrawals (0500–0599) ===
        public const string InvalidWithdrawalMethod = "RealInvest:0500";
        public const string WithdrawalNotFound = "RealInvest:0501";
        public const string WithdrawalCannotBeModified = "RealInvest:0502";
        public const string WithdrawalExceedsAvailable = "RealInvest:0503";
        public const string WithdrawalInsufficientForFee = "RealInvest:0504";
        public const string InvalidBankAccountDetails = "RealInvest:0505";
        public const string InvalidVodafoneCashNumber = "RealInvest:0506";

        // === Offerings (0600–0699) ===
        public const string OfferingNotFound = "RealInvest:0600";
        public const string OfferingNotOpen = "RealInvest:0601";
        public const string OfferingClosed = "RealInvest:0602";
        public const string OfferingNoSharesAvailable = "RealInvest:0603";
        public const string InvalidRiskLevel = "RealInvest:0604";
        public const string OfferingCannotBeModified = "RealInvest:0605";

        // === Investment Orders (0700–0799) ===
        public const string OrderNotFound = "RealInvest:0700";
        public const string OrderInvalidState = "RealInvest:0701";
        public const string OrderInsufficientFunds = "RealInvest:0702";
        public const string OrderCannotBeModified = "RealInvest:0703";
        public const string OrderAlreadySettled = "RealInvest:0704";
        public const string OrderAlreadyFailed = "RealInvest:0705";
        public const string InvalidOrderAmount = "RealInvest:0706";
        public const string KnowledgeCheckRequired = "RealInvest:0707";
        public const string KnowledgeCheckFailed = "RealInvest:0708";
        public const string KnowledgeCheckExpired = "RealInvest:0709";

        // === Installments (0800–0899) ===
        public const string InstallmentNotFound = "RealInvest:0800";
        public const string InstallmentAlreadyPaid = "RealInvest:0801";
        public const string InstallmentInsufficientFunds = "RealInvest:0802";
        public const string InstallmentDueDateInvalid = "RealInvest:0803";

        // === Property Sales & Exit (0900–0999) ===
        public const string PropertySaleNotFound = "RealInvest:0900";
        public const string PropertySaleAlreadyInitiated = "RealInvest:0901";
        public const string SaleVoteNotFound = "RealInvest:0902";
        public const string SaleVoteAlreadyExists = "RealInvest:0903";
        public const string InvalidSalePrice = "RealInvest:0904";
        public const string SaleCannotBeSettled = "RealInvest:0905";
        public const string HoldingNotFound = "RealInvest:0906";

        // === Ledger & Transactions (1000–1099) ===
        public const string IdempotencyKeyDuplicate = "RealInvest:1000";
        public const string LedgerEntryNotFound = "RealInvest:1001";
        public const string LedgerBalanceMismatch = "RealInvest:1002";
        public const string LedgerEntryCannotBeModified = "RealInvest:1003";
        public const string TransactionAlreadyProcessed = "RealInvest:1004";

        // === Support & Tickets (1100–1199) ===
        public const string TicketNotFound = "RealInvest:1100";
        public const string TicketAlreadyClosed = "RealInvest:1101";
        public const string InvalidTicketCategory = "RealInvest:1102";
        public const string TicketAttachmentFailed = "RealInvest:1103";
        public const string MaxAttachmentsExceeded = "RealInvest:1104";

        // === Admin Operations (1200–1299) ===
        public const string AdminPermissionDenied = "RealInvest:1200";
        public const string AdminOperationFailed = "RealInvest:1201";
        public const string AdminAuditFailed = "RealInvest:1202";

        // === Configuration & System (1300–1399) ===
        public const string FeePolicyNotFound = "RealInvest:1300";
        public const string FeePolicyInvalid = "RealInvest:1301";
        public const string ConfigurationMissing = "RealInvest:1302";
    }
}

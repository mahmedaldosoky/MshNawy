namespace MshNawy.Domain.Shared
{
    /// <summary>
    /// MshNawy error codes for API responses and domain exceptions.
    /// Format: "MshNawy:NNNN"
    /// </summary>
    public static class MshNawyErrorCodes
    {
        // === Generic (0001–0099) ===
        public const string DefaultError = "MshNawy:0001";
        public const string InvalidInput = "MshNawy:0002";
        public const string UnexpectedError = "MshNawy:0003";
        public const string Unauthorized = "MshNawy:0004";
        public const string Forbidden = "MshNawy:0005";
        public const string NotFound = "MshNawy:0006";
        public const string ValidationFailed = "MshNawy:0007";
        public const string ConcurrencyFailed = "MshNawy:0008";

        // === OTP & Authentication (0100–0199) ===
        public const string InvalidPhoneFormat = "MshNawy:0100";
        public const string OtpRequestFailed = "MshNawy:0101";
        public const string OtpInvalid = "MshNawy:0102";
        public const string OtpExpired = "MshNawy:0103";
        public const string OtpRateLimited = "MshNawy:0104";
        public const string OtpPhoneLockedOut = "MshNawy:0105";
        public const string InvalidCredentials = "MshNawy:0106";
        public const string SessionExpired = "MshNawy:0107";

        // === KYC Verification (0200–0299) ===
        public const string KycNotStarted = "MshNawy:0200";
        public const string KycNotApproved = "MshNawy:0201";
        public const string KycPending = "MshNawy:0202";
        public const string KycRejected = "MshNawy:0203";
        public const string KycNeedsResubmission = "MshNawy:0204";
        public const string KycInvalidNationalId = "MshNawy:0205";
        public const string KycImageUploadFailed = "MshNawy:0206";
        public const string KycImageInvalid = "MshNawy:0207";
        public const string KycUnderReview = "MshNawy:0208";

        // === Wallet & Balance (0300–0399) ===
        public const string InsufficientBalance = "MshNawy:0300";
        public const string InsufficientAvailableBalance = "MshNawy:0301";
        public const string InvalidAmount = "MshNawy:0302";
        public const string NegativeAmount = "MshNawy:0303";
        public const string AmountExceedsMaximum = "MshNawy:0304";
        public const string WalletNotFound = "MshNawy:0305";

        // === Deposits (0400–0499) ===
        public const string InvalidDepositMethod = "MshNawy:0400";
        public const string DepositAlreadyProcessed = "MshNawy:0401";
        public const string DepositProofInvalid = "MshNawy:0402";
        public const string DepositProofMissing = "MshNawy:0403";
        public const string DepositReferenceMissing = "MshNawy:0404";
        public const string DepositNotFound = "MshNawy:0405";
        public const string DepositCannotBeModified = "MshNawy:0406";
        public const string DepositAmountMismatch = "MshNawy:0407";

        // === Withdrawals (0500–0599) ===
        public const string InvalidWithdrawalMethod = "MshNawy:0500";
        public const string WithdrawalNotFound = "MshNawy:0501";
        public const string WithdrawalCannotBeModified = "MshNawy:0502";
        public const string WithdrawalExceedsAvailable = "MshNawy:0503";
        public const string WithdrawalInsufficientForFee = "MshNawy:0504";
        public const string InvalidBankAccountDetails = "MshNawy:0505";
        public const string InvalidVodafoneCashNumber = "MshNawy:0506";

        // === Offerings (0600–0699) ===
        public const string OfferingNotFound = "MshNawy:0600";
        public const string OfferingNotOpen = "MshNawy:0601";
        public const string OfferingClosed = "MshNawy:0602";
        public const string OfferingNoSharesAvailable = "MshNawy:0603";
        public const string InvalidRiskLevel = "MshNawy:0604";
        public const string OfferingCannotBeModified = "MshNawy:0605";

        // === Investment Orders (0700–0799) ===
        public const string OrderNotFound = "MshNawy:0700";
        public const string OrderInvalidState = "MshNawy:0701";
        public const string OrderInsufficientFunds = "MshNawy:0702";
        public const string OrderCannotBeModified = "MshNawy:0703";
        public const string OrderAlreadySettled = "MshNawy:0704";
        public const string OrderAlreadyFailed = "MshNawy:0705";
        public const string InvalidOrderAmount = "MshNawy:0706";
        public const string KnowledgeCheckRequired = "MshNawy:0707";
        public const string KnowledgeCheckFailed = "MshNawy:0708";
        public const string KnowledgeCheckExpired = "MshNawy:0709";

        // === Installments (0800–0899) ===
        public const string InstallmentNotFound = "MshNawy:0800";
        public const string InstallmentAlreadyPaid = "MshNawy:0801";
        public const string InstallmentInsufficientFunds = "MshNawy:0802";
        public const string InstallmentDueDateInvalid = "MshNawy:0803";

        // === Property Sales & Exit (0900–0999) ===
        public const string PropertySaleNotFound = "MshNawy:0900";
        public const string PropertySaleAlreadyInitiated = "MshNawy:0901";
        public const string SaleVoteNotFound = "MshNawy:0902";
        public const string SaleVoteAlreadyExists = "MshNawy:0903";
        public const string InvalidSalePrice = "MshNawy:0904";
        public const string SaleCannotBeSettled = "MshNawy:0905";
        public const string HoldingNotFound = "MshNawy:0906";

        // === Ledger & Transactions (1000–1099) ===
        public const string IdempotencyKeyDuplicate = "MshNawy:1000";
        public const string LedgerEntryNotFound = "MshNawy:1001";
        public const string LedgerBalanceMismatch = "MshNawy:1002";
        public const string LedgerEntryCannotBeModified = "MshNawy:1003";
        public const string TransactionAlreadyProcessed = "MshNawy:1004";

        // === Support & Tickets (1100–1199) ===
        public const string TicketNotFound = "MshNawy:1100";
        public const string TicketAlreadyClosed = "MshNawy:1101";
        public const string InvalidTicketCategory = "MshNawy:1102";
        public const string TicketAttachmentFailed = "MshNawy:1103";
        public const string MaxAttachmentsExceeded = "MshNawy:1104";

        // === Admin Operations (1200–1299) ===
        public const string AdminPermissionDenied = "MshNawy:1200";
        public const string AdminOperationFailed = "MshNawy:1201";
        public const string AdminAuditFailed = "MshNawy:1202";

        // === Configuration & System (1300–1399) ===
        public const string FeePolicyNotFound = "MshNawy:1300";
        public const string FeePolicyInvalid = "MshNawy:1301";
        public const string ConfigurationMissing = "MshNawy:1302";
    }
}

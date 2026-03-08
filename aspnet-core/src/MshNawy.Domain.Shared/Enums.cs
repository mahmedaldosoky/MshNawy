namespace MshNawy.Domain.Shared
{
    public enum KycStatus
    {
        Draft = 0,
        Submitted = 1,
        UnderReview = 2,
        Approved = 3,
        Rejected = 4,
        NeedsResubmission = 5
    }

    public enum DepositMethod
    {
        InstaPay = 0,
        VodafoneCash = 1,
        BankTransfer = 2
    }

    public enum DepositStatus
    {
        Created = 0,
        PendingReview = 1,
        Approved = 2,
        Rejected = 3,
        Posted = 4
    }

    public enum WithdrawalMethod
    {
        BankTransfer = 0,
        VodafoneCash = 1
    }

    public enum WithdrawalStatus
    {
        Created = 0,
        PendingReview = 1,
        Processing = 2,
        Completed = 3,
        Rejected = 4
    }

    public enum RiskLevel
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public enum OfferingStatus
    {
        Draft = 0,
        Open = 1,
        Closed = 2,
        SaleInitiated = 3,
        Settled = 4
    }

    public enum OrderStatus
    {
        Created = 0,
        Reserved = 1,
        Submitted = 2,
        Settled = 3,
        Failed = 4
    }

    public enum InstallmentStatus
    {
        Upcoming = 0,
        Due = 1,
        Paid = 2,
        Overdue = 3,
        GracePeriod = 4,
        Flagged = 5,
        Cancelled = 6
    }

    public enum SaleVoteDirection
    {
        Sell = 0,
        Hold = 1
    }

    public enum SaleTriggerType
    {
        MajorityVote = 0,
        Maturity = 1,
        PlatformDecision = 2
    }

    public enum PropertySaleStatus
    {
        Initiated = 0,
        Listed = 1,
        Sold = 2,
        Distributing = 3,
        Settled = 4,
        Cancelled = 5
    }

    public enum TicketCategory
    {
        General = 0,
        Technical = 1,
        Financial = 2,
        KYC = 3,
        Complaint = 4
    }

    public enum TicketStatus
    {
        Open = 0,
        InProgress = 1,
        Resolved = 2,
        Closed = 3
    }

    public enum LedgerEntryType
    {
        Deposit = 0,
        Withdrawal = 1,
        OrderReservation = 2,
        OrderSettlement = 3,
        OrderFailure = 4,
        InstallmentPayment = 5,
        SaleProceeds = 6,
        SaleExitFee = 7,
        FeeCollection = 8,
        Compensating = 9
    }

    public enum NotificationEventType
    {
        KycStatusChange = 0,
        DepositPosted = 1,
        DepositRejected = 2,
        WithdrawalCompleted = 3,
        WithdrawalRejected = 4,
        OrderSettled = 5,
        OrderFailed = 6,
        InstallmentDue = 7,
        InstallmentOverdue = 8,
        SaleVoteThresholdReached = 9,
        PropertySaleUpdate = 10,
        SaleProceedsDistributed = 11,
        TicketResponse = 12
    }
}

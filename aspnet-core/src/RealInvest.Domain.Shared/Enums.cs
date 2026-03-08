namespace RealInvest.Domain.Shared
{
    public enum KycStatus { Draft, Submitted, UnderReview, Approved, Rejected, NeedsResubmission }
    public enum DepositMethod { InstaPay, VodafoneCash, BankTransfer }
    public enum DepositStatus { Created, PendingReview, Approved, Rejected, Posted }
    public enum WithdrawalMethod { BankTransfer, VodafoneCash }
    public enum WithdrawalStatus { Created, PendingReview, Processing, Completed, Rejected }
    public enum RiskLevel { Low, Medium, High }
    public enum OfferingStatus { Draft, Open, Closed, SaleInitiated, Settled }
    public enum OrderStatus { Created, Reserved, Submitted, Settled, Failed }
    public enum InstallmentStatus { Upcoming, Due, Paid, Overdue, GracePeriod, Flagged, Cancelled }
    public enum SaleVoteDirection { Sell, Hold }
    public enum SaleTriggerType { MajorityVote, Maturity, PlatformDecision }
    public enum PropertySaleStatus { Initiated, Listed, Sold, Distributing, Settled, Cancelled }
    public enum TicketCategory { General, Technical, Financial, KYC, Complaint }
    public enum TicketStatus { Open, InProgress, Resolved, Closed }
    public enum LedgerEntryType { Deposit, Withdrawal, OrderReservation, OrderSettlement, OrderFailure, InstallmentPayment, SaleProceeds, SaleExitFee, FeeCollection, Compensating }
    public enum NotificationEventType { KycStatusChange, DepositPosted, DepositRejected, WithdrawalCompleted, WithdrawalRejected, OrderSettled, OrderFailed, InstallmentDue, InstallmentOverdue, SaleVoteThresholdReached, PropertySaleUpdate, SaleProceedsDistributed, TicketResponse }
}

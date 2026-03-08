using System;

namespace MshNawy.Domain.Wallet
{
    /// <summary>
    /// Immutable ledger entry - record of a single monetary movement.
    /// Per Constitution II: All monetary movements recorded via double-entry (debit/credit pair).
    /// </summary>
    public class LedgerEntry
    {
        public Guid Id { get; private set; }
        public DateTime Timestamp { get; private set; }
        public MshNawy.Domain.Shared.LedgerEntryType EntryType { get; private set; }
        public string? DebitAccount { get; private set; }
        public string? CreditAccount { get; private set; }
        public long Amount { get; private set; }
        public Guid IdempotencyKey { get; private set; }
        public string? ReferenceEntityType { get; private set; }
        public Guid ReferenceEntityId { get; private set; }
        public string? Description { get; private set; }
        public Guid? CompensatingEntryId { get; set; } // Settable for compensating entry linking
        public Guid? PostedByUserId { get; private set; }

        /// <summary>
        /// Parameterless constructor for Entity Framework
        /// </summary>
        private LedgerEntry() { }

        /// <summary>
        /// Create a new ledger entry
        /// </summary>
        public LedgerEntry(
            Guid id,
            DateTime timestamp,
            MshNawy.Domain.Shared.LedgerEntryType entryType,
            string debitAccount,
            string creditAccount,
            long amount,
            Guid idempotencyKey,
            string referenceEntityType,
            Guid referenceEntityId,
            string description)
        {
            Id = id;
            Timestamp = timestamp;
            EntryType = entryType;
            DebitAccount = debitAccount;
            CreditAccount = creditAccount;
            Amount = amount;
            IdempotencyKey = idempotencyKey;
            ReferenceEntityType = referenceEntityType;
            ReferenceEntityId = referenceEntityId;
            Description = description;
        }
    }
}

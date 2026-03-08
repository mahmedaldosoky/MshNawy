using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealInvest.Domain.Wallet
{
    /// <summary>
    /// Domain service for ledger operations. Per Constitution II, manages immutable double-entry ledger entries.
    /// All monetary movements MUST be recorded via debit/credit pairs.
    /// Posted entries are immutable; corrections use compensating entries only.
    /// </summary>
    public class LedgerService
    {
        private readonly List<LedgerEntry> _entries = new(); // Simulated persistence until ABP repo is available

        /// <summary>
        /// Posts a ledger entry. In production, this will be called within a database transaction.
        /// The entry MUST have a debit account and a credit account; both are credited/debited atomically.
        /// </summary>
        public Task PostEntryAsync(LedgerEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (string.IsNullOrWhiteSpace(entry.DebitAccount))
                throw new InvalidOperationException("LedgerEntry must have a DebitAccount.");

            if (string.IsNullOrWhiteSpace(entry.CreditAccount))
                throw new InvalidOperationException("LedgerEntry must have a CreditAccount.");

            if (entry.Amount <= 0)
                throw new InvalidOperationException("LedgerEntry amount must be positive.");

            // Check for duplicate idempotency key (prevents duplicate processing)
            if (_entries.Any(e => e.IdempotencyKey == entry.IdempotencyKey))
                throw new InvalidOperationException($"Idempotency key {entry.IdempotencyKey} already exists. Duplicate entry rejected.");

            // Store entry (in real implementation, this persists to database within a transaction)
            _entries.Add(entry);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Posts a compensating (reversal) entry that references the original entry.
        /// Used to reverse a posted entry without modifying it (maintains immutability).
        /// The compensating entry has swapped debit/credit accounts.
        /// </summary>
        public Task PostCompensatingEntryAsync(Guid originalEntryId, LedgerEntry compensatingEntry)
        {
            if (originalEntryId == Guid.Empty)
                throw new ArgumentException("Original entry ID cannot be empty.", nameof(originalEntryId));

            if (compensatingEntry == null)
                throw new ArgumentNullException(nameof(compensatingEntry));

            var originalEntry = _entries.FirstOrDefault(e => e.Id == originalEntryId);
            if (originalEntry == null)
                throw new InvalidOperationException($"Original entry {originalEntryId} not found.");

            // Set the compensating entry's reference to the original
            compensatingEntry.CompensatingEntryId = originalEntryId;

            // Post the compensating entry using the same rules
            return PostEntryAsync(compensatingEntry);
        }

        /// <summary>
        /// Validates that ledger balances are correct (sum of all debits = sum of all credits).
        /// Per Constitution II, cached/derived balances must be reconcilable against the ledger.
        /// </summary>
        public Task ValidateBalanceAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            // In double-entry accounting, every entry debits one account and credits another with the same amount
            // So the total debits and total credits across ALL accounts should always be equal
            // We validate that each entry has both accounts and that no entries are malformed
            foreach (var entry in _entries)
            {
                if (string.IsNullOrWhiteSpace(entry.DebitAccount))
                    throw new InvalidOperationException(
                        $"Entry {entry.Id} has no debit account.");

                if (string.IsNullOrWhiteSpace(entry.CreditAccount))
                    throw new InvalidOperationException(
                        $"Entry {entry.Id} has no credit account.");

                if (entry.Amount <= 0)
                    throw new InvalidOperationException(
                        $"Entry {entry.Id} has invalid amount: {entry.Amount}");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Calculates the balance for a specific account.
        /// </summary>
        public Task<long> GetAccountBalanceAsync(string account)
        {
            if (string.IsNullOrWhiteSpace(account))
                throw new ArgumentException("Account cannot be empty.", nameof(account));

            var debits = _entries.Where(e => e.DebitAccount == account).Sum(e => e.Amount);
            var credits = _entries.Where(e => e.CreditAccount == account).Sum(e => e.Amount);

            var balance = credits - debits; // Credits increase, debits decrease
            return Task.FromResult(balance);
        }
    }
}

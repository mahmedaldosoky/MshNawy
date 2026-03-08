using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealInvest.Domain.Wallet
{
    /// <summary>
    /// Domain service that derives user wallet balances from ledger entries.
    /// Per Constitution II, balances are derived from the ledger at any point in time.
    /// Four balance types: Available, Reserved, Invested, PendingWithdrawal.
    /// </summary>
    public class BalanceCalculator
    {
        private readonly LedgerService _ledgerService;

        public BalanceCalculator(LedgerService ledgerService)
        {
            _ledgerService = ledgerService ?? throw new ArgumentNullException(nameof(ledgerService));
        }

        /// <summary>
        /// Calculates wallet balances for a user by summing ledger entries across four account types.
        /// Returns tuple: (Available, Reserved, Invested, PendingWithdrawal)
        ///
        /// Account naming convention (from data-model.md):
        /// - "User:{userId}:Available" — funds available for investment or withdrawal
        /// - "User:{userId}:Reserved" — funds temporarily held during order creation
        /// - "User:{userId}:Invested" — funds committed to completed investments
        /// - "User:{userId}:PendingWithdrawal" — funds pending withdrawal approval
        /// </summary>
        public async Task<(long Available, long Reserved, long Invested, long PendingWithdrawal)> GetBalancesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            var available = await _ledgerService.GetAccountBalanceAsync($"User:{userId}:Available");
            var reserved = await _ledgerService.GetAccountBalanceAsync($"User:{userId}:Reserved");
            var invested = await _ledgerService.GetAccountBalanceAsync($"User:{userId}:Invested");
            var pendingWithdrawal = await _ledgerService.GetAccountBalanceAsync($"User:{userId}:PendingWithdrawal");

            return (available, reserved, invested, pendingWithdrawal);
        }

        /// <summary>
        /// Calculates total wallet value (sum of all four balances).
        /// </summary>
        public async Task<long> GetTotalWalletValueAsync(Guid userId)
        {
            var (available, reserved, invested, pendingWithdrawal) = await GetBalancesAsync(userId);
            return available + reserved + invested + pendingWithdrawal;
        }

        /// <summary>
        /// Checks if a user has sufficient available balance for a transaction.
        /// </summary>
        public async Task<bool> HasSufficientBalanceAsync(Guid userId, long requiredAmount)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            if (requiredAmount <= 0)
                throw new ArgumentException("Required amount must be positive.", nameof(requiredAmount));

            var (available, _, _, _) = await GetBalancesAsync(userId);
            return available >= requiredAmount;
        }
    }
}

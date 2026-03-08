using System;
using System.Threading.Tasks;

namespace RealInvest.Domain.Fees
{
    public class FeeCalculator
    {
        /// <summary>
        /// Calculates entry fee using integer arithmetic (no floating-point) per Constitution II.
        /// Formula: fee = (amount * percent) / 100, rounded down.
        /// </summary>
        public Task<long> CalculateEntryFeeAsync(long amountPiasters, FeePolicy policy)
        {
            var fee = (long)((amountPiasters * policy.EntryFeePercent) / 100);
            return Task.FromResult(fee);
        }

        /// <summary>
        /// Calculates per-payment fee using integer arithmetic per Constitution II.
        /// </summary>
        public Task<long> CalculatePaymentFeeAsync(long amountPiasters, FeePolicy policy)
        {
            var fee = (long)((amountPiasters * policy.PaymentFeePercent) / 100);
            return Task.FromResult(fee);
        }

        /// <summary>
        /// Calculates exit fee (brokerage + platform) using integer arithmetic per Constitution II.
        /// Returns tuple: (Brokerage, Platform).
        /// </summary>
        public Task<(long Brokerage, long Platform)> CalculateExitFeeAsync(long amountPiasters, FeePolicy policy)
        {
            var brokerage = (long)((amountPiasters * policy.ExitBrokeragePercent) / 100);
            var platform = (long)((amountPiasters * policy.ExitPlatformProfitPercent) / 100);
            return Task.FromResult((brokerage, platform));
        }
    }
}

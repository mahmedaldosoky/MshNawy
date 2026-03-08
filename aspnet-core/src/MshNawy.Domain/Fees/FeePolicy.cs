using System;

namespace MshNawy.Domain.Fees
{
    public class FeePolicy
    {
        public Guid Id { get; private set; }
        public int Version { get; private set; }
        public DateTime EffectiveFrom { get; private set; }
        public DateTime? EffectiveTo { get; private set; }
        public decimal EntryFeePercent { get; private set; }
        public decimal PaymentFeePercent { get; private set; }
        public decimal ExitFeePercent { get; private set; }
        public decimal ExitBrokeragePercent { get; private set; }
        public decimal ExitPlatformProfitPercent { get; private set; }

        private FeePolicy() { }

        public FeePolicy(Guid id, int version, DateTime effectiveFrom, decimal entryFeePercent, decimal paymentFeePercent, decimal exitFeePercent, decimal exitBrokeragePercent, decimal exitPlatformProfitPercent)
        {
            Id = id;
            Version = version;
            EffectiveFrom = effectiveFrom;
            EntryFeePercent = entryFeePercent;
            PaymentFeePercent = paymentFeePercent;
            ExitFeePercent = exitFeePercent;
            ExitBrokeragePercent = exitBrokeragePercent;
            ExitPlatformProfitPercent = exitPlatformProfitPercent;
        }
    }
}

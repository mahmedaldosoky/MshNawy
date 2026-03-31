using System.Threading.Tasks;

namespace MshNawy.Domain.Fees;

public interface IFeeCalculator
{
    Task<long> CalculateEntryFeeAsync(long amountPiasters, FeePolicy policy);
    Task<long> CalculatePaymentFeeAsync(long amountPiasters, FeePolicy policy);
    Task<(long Brokerage, long Platform)> CalculateExitFeeAsync(long amountPiasters, FeePolicy policy);
}

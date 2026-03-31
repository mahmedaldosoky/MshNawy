using System;
using System.Threading.Tasks;

namespace MshNawy.Domain.Wallet;

public interface IBalanceCalculator
{
    Task<(long Available, long Reserved, long Invested, long PendingWithdrawal)> GetBalancesAsync(Guid userId);
    Task<long> GetTotalWalletValueAsync(Guid userId);
    Task<bool> HasSufficientBalanceAsync(Guid userId, long requiredAmount);
}

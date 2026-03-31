using System;
using System.Threading.Tasks;

namespace MshNawy.Domain.Wallet;

public interface ILedgerService
{
    Task PostEntryAsync(LedgerEntry entry);
    Task PostCompensatingEntryAsync(Guid originalEntryId, LedgerEntry compensatingEntry);
    Task ValidateBalanceAsync(Guid userId);
    Task<long> GetAccountBalanceAsync(string account);
}

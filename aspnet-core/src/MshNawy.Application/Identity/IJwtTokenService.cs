using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace MshNawy.Application.Identity;

public interface IJwtTokenService
{
    Task<string> CreateAccessTokenAsync(IdentityUser user);
}

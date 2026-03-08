using Volo.Abp.Modularity;

namespace RealInvest.Domain.Shared
{
    /// <summary>
    /// RealInvest Domain Shared Module - registers shared constants, enums, and domain objects.
    /// Per Constitution VII: Core module for shared domain concepts across layers.
    /// Per Constitution I: Arabic-only localization (configured in HttpApi.Host).
    /// </summary>
    public class RealInvestDomainSharedModule : AbpModule
    {
        // Shared module - no special configuration needed at this layer
        // Localization is configured at the HttpApi.Host level
    }

    /// <summary>
    /// Localization resource for RealInvest - provides base for all localization strings
    /// Per Constitution I: All user-facing messages in Arabic only
    /// </summary>
    public class RealInvestResource { }
}

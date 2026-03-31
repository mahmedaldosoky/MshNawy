using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Timing;

namespace MshNawy.Application.Identity;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions options;
    private readonly IdentityUserManager userManager;
    private readonly IClock clock;

    public JwtTokenService(IOptions<JwtOptions> options, IdentityUserManager userManager, IClock clock)
    {
        this.options = options.Value;
        this.userManager = userManager;
        this.clock = clock;
    }

    public async Task<string> CreateAccessTokenAsync(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
            new Claim(AbpClaimTypes.UserName, user.UserName ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
        {
            claims.Add(new Claim(AbpClaimTypes.PhoneNumber, user.PhoneNumber));
        }

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(AbpClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = clock.Now.AddMinutes(options.ExpiresInMinutes);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

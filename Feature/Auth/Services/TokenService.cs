namespace AspNetEmptyApp.Feature.Auth.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNetEmptyApp.Feature.Auth.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class TokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public TokenService(
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<(string Token, DateTime ExpiresAt, IList<string> Roles, IList<string> Permissions)> GenerateTokenAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            var identityRole = await _roleManager.FindByNameAsync(role);
            if (identityRole != null)
            {
                var roleClaims = await _roleManager.GetClaimsAsync(identityRole);
                foreach (var rc in roleClaims)
                {
                    if (!claims.Any(c => c.Type == rc.Type && c.Value == rc.Value))
                    {
                        claims.Add(rc);
                    }
                }
            }
        }

        foreach (var uc in userClaims)
        {
            if (!claims.Any(c => c.Type == uc.Type && c.Value == uc.Value))
            {
                claims.Add(uc);
            }
        }

        var jwtKey = _configuration["Jwt:Key"] ?? "SuperSecretKeyForJwtTokenAuth1234567890!@#$%";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(4);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "AspNetEmptyApp",
            audience: _configuration["Jwt:Audience"] ?? "AspNetEmptyApp",
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var permissions = claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

        return (new JwtSecurityTokenHandler().WriteToken(token), expires, roles, permissions);
    }
}

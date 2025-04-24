using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ModelServer.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API_Server;

public class JwtHandler(IConfiguration configuration, UserManager<CarAppUser> userManager)
{
    public async Task<JwtSecurityToken> GetTokenAsync(CarAppUser user) =>
        new(
            issuer: configuration["JwtSettings:Issuer"],
            audience: configuration["JwtSettings:Audience"],
            claims: await GetClaimsAsync(user),
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(configuration["JwtSettings:ExpirationTimeInMinutes"])),
            signingCredentials: GetSigningCredentials());

    private SigningCredentials GetSigningCredentials()
    {
        byte[] key = Encoding.UTF8.GetBytes(configuration["JwtSettings:SecurityKey"]!);
        return new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaimsAsync(CarAppUser user)
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, user.UserName!) };
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        return claims;
    }
}

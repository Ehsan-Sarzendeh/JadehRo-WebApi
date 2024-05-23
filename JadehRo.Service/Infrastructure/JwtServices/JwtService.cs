using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using JadehRo.Database.Entities.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JadehRo.Service.Infrastructure.JwtServices;

public class JwtService : IJwtService
{
    private readonly SiteSettings _siteSetting;
    private readonly SignInManager<User> _signInManager;


    public JwtService(IOptionsSnapshot<SiteSettings> settings, SignInManager<User> signInManager)
    {
        _siteSetting = settings.Value;
        _signInManager = signInManager;
    }

    public async Task<AccessToken> GenerateTokenAsync(User user, List<Claim> customClaims = null)
    {
        var secretKey = Encoding.UTF8.GetBytes(_siteSetting.JwtSettings.SecretKey); // longer that 16 character
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

        var claims = await GetClaimsAsync(user, customClaims);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _siteSetting.JwtSettings.Issuer,
            Audience = _siteSetting.JwtSettings.Audience,
            IssuedAt = DateTime.Now,
            NotBefore = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.NotBeforeMinutes),
            Expires = DateTime.Now.AddMinutes(_siteSetting.JwtSettings.ExpirationMinutes),
            SigningCredentials = signingCredentials,
            Subject = new ClaimsIdentity(claims),
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

        return new AccessToken(securityToken);
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(User user, List<Claim> customClaims)
    {
        var result = await _signInManager.ClaimsFactory.CreateAsync(user);
        var claims = new List<Claim>(result.Claims);

        if (customClaims is { Count: > 0 })
            claims.AddRange(customClaims);

        claims.Add(new Claim("UserType", Convert.ToString((int)user.Type)));

        return claims;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
using System.Security.Claims;
using JadehRo.Common.Utilities;
using JadehRo.Database.Entities.Users;

namespace JadehRo.Service.Infrastructure.JwtServices;

public interface IJwtService : IScopedDependency
{
    Task<AccessToken> GenerateTokenAsync(User user, List<Claim> customClaims = null);
    string GenerateRefreshToken();
}
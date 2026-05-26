using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using GarageInventoryPartsFinder.Application.Authentication;
using GarageInventoryPartsFinder.Domain.Entities;

namespace GarageInventoryPartsFinder.Api.Authentication;

public sealed class TokenService(IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public string CreateAccessToken(AuthUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.DisplayName)
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetAccessTokenExpiryUtc() => DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);

    public DateTime GetRefreshTokenExpiryUtc() => DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays);

    public string CreateRefreshToken() => Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));

    public string HashRefreshToken(string refreshToken)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(hash);
    }

    public AuthResponse CreateResponse(AuthUser user, string refreshToken)
    {
        return new AuthResponse(
            user.Id,
            user.Email,
            user.DisplayName,
            CreateAccessToken(user),
            refreshToken,
            GetAccessTokenExpiryUtc(),
            GetRefreshTokenExpiryUtc());
    }
}

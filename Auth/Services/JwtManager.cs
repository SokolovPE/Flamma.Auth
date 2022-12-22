using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Flamma.Auth.Common.Interfaces;
using Flamma.Auth.Interfaces;
using Flamma.Auth.Models;
using Microsoft.IdentityModel.Tokens;

namespace Flamma.Auth.Services;

/// <inheritdoc />
public class JwtManager : IJwtManager
{
    /// <summary>
    ///     Application configuration
    /// </summary>
    private readonly IConfiguration _configuration;
    
    /// <summary>
    ///     Date provider
    /// </summary>
    private readonly IDateProvider _dateProvider;

    /// <summary>
    ///     .ctor
    /// </summary>
    public JwtManager(IConfiguration configuration, IDateProvider dateProvider)
    {
        _configuration = configuration;
        _dateProvider = dateProvider;
    }
    
    /// <inheritdoc />
    public JwtTokenInfo GenerateToken(string username)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        return GenerateToken(authClaims);
    }

    /// <inheritdoc />
    public JwtTokenInfo RefreshToken(string token)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        return GenerateToken(principal.Claims);
    }

    /// <inheritdoc />
    public string ExtractUsername(string token)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        return principal.Identity?.Name;
    }

    /// <summary>
    ///     Create token from claims
    /// </summary>
    private JwtSecurityToken CreateToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        _ = int.TryParse(_configuration["Jwt:TokenValidityInMinutes"], out var tokenValidityInMinutes);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            expires: _dateProvider.UtcNow.AddMinutes(tokenValidityInMinutes),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }

    /// <summary>
    ///     Extract principal from expired token
    /// </summary>
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"])),
            ValidateLifetime = false
        };
        
        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Token is invalid");
        return principal;
    }
    
    /// <summary>
    ///     Generate token info using claims
    /// </summary>
    private JwtTokenInfo GenerateToken(IEnumerable<Claim> claims)
    {
        var token = CreateToken(claims);
        var refreshToken = GenerateRefreshToken();

        var tokenInfo = new JwtTokenInfo
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            TokenValidTo = token.ValidTo
        };

        return tokenInfo;
    }
    
    /// <summary>
    ///     Generate random refresh token
    /// </summary>
    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
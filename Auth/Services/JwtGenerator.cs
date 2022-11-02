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
public class JwtGenerator : IJwtGenerator
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
    public JwtGenerator(IConfiguration configuration, IDateProvider dateProvider)
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

        var token = CreateToken(authClaims);
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
    ///     Create token from claims
    /// </summary>
    private JwtSecurityToken CreateToken(List<Claim> authClaims)
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
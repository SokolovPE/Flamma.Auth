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

    /// <inheritdoc />
    public int GetTokenValidityCheckPeriod() => int.Parse(_configuration["Jwt:TokenValidityCheckPeriodInSeconds"]);

    /// <inheritdoc />
    public JwtTokenInfo RefreshToken(string token, string refreshToken)
    {
        // TODO: Read how to generate refresh token
        var newToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        return new JwtTokenInfo
        {
            Token = new JwtSecurityTokenHandler().WriteToken(newToken),
            RefreshToken = refreshToken,
            TokenValidTo = newToken.ValidTo
        };
    }

    /// <inheritdoc />
    public JwtTokenStatus ValidateToken(string token, string username)
    {
        var handler = new JwtSecurityTokenHandler();
        var parsedToken = handler.ReadJwtToken(token);
        
        // Check token sign
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        if (parsedToken.SigningCredentials.Key != authSigningKey ||
            parsedToken.SigningCredentials.Algorithm != SecurityAlgorithms.HmacSha256)
            return JwtTokenStatus.Invalid;

        // Check if token belongs to this user
        if (parsedToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value != username)
            return JwtTokenStatus.Invalid;
        
        // Check issuer and audience
        if (parsedToken.Audiences.Contains(_configuration["Jwt:Issuer"]) &&
            parsedToken.Issuer == _configuration["Jwt:Audience"])
            return JwtTokenStatus.Invalid;

        // Check time token valid to
        return parsedToken.ValidTo < _dateProvider.UtcNow ? JwtTokenStatus.Expired : JwtTokenStatus.Valid;
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
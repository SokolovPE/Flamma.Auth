using Flamma.Auth.Models;

namespace Flamma.Auth.Interfaces;

/// <summary>
///     Service which operates jwt token operations
/// </summary>
public interface IJwtGenerator
{
    /// <summary>
    ///     Generate token for user
    /// </summary>
    public JwtTokenInfo GenerateToken(string username);

    /// <summary>
    ///     Validate jwt token
    /// </summary>
    JwtTokenStatus ValidateToken(string token, string username);

    /// <summary>
    ///     Get period of jwt token validation
    /// </summary>
    /// <returns>Period of validation in seconds</returns>
    int GetTokenValidityCheckPeriod();

    /// <summary>
    ///     Refresh given token
    /// </summary>
    JwtTokenInfo RefreshToken(string token, string refreshToken);
}
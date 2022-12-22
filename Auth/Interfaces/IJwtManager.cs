using Flamma.Auth.Models;

namespace Flamma.Auth.Interfaces;

/// <summary>
///     Service which operates jwt token operations
/// </summary>
public interface IJwtManager
{
    /// <summary>
    ///     Generate token for user
    /// </summary>
    public JwtTokenInfo GenerateToken(string username);

    /// <summary>
    ///     Refresh given token
    /// </summary>
    JwtTokenInfo RefreshToken(string token);

    /// <summary>
    ///     Extract username from token
    /// </summary>
    string ExtractUsername(string token);
}
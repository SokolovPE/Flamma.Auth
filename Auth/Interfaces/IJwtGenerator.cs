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
}
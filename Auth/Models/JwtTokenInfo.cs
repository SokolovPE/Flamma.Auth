namespace Flamma.Auth.Models;

/// <summary>
///     Generated Jwt token with additional information
/// </summary>
public class JwtTokenInfo
{
    /// <summary>
    ///     Token itself
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    ///     Refresh token
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    ///     Token expiration time
    /// </summary>
    public DateTime TokenValidTo { get; set; }
}
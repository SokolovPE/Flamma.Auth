namespace Flamma.Auth.Models;

/// <summary>
///     Result of user login action
/// </summary>
public class LoginResult
{
    /// <summary>
    ///     Indicates was registration successful or not
    /// </summary>
    public bool Success { get; set; }
    
    /// <summary>
    ///     Token information
    /// </summary>
    public JwtTokenInfo TokenInfo { get; set; }
}
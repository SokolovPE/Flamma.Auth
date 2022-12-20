namespace Flamma.Auth.Models;

/// <summary>
///     Possible statuses of jwt token
/// </summary>
public enum JwtTokenStatus : byte
{
    /// <summary>
    ///     Token is fully valid
    /// </summary>
    Valid = 1,
    
    /// <summary>
    ///     Token is valid but expired
    /// </summary>
    Expired = 2,
    
    /// <summary>
    ///     Token is invalid
    /// </summary>
    Invalid = 3
}
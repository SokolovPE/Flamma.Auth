namespace Flamma.Auth.Models;

/// <summary>
///     User information
/// </summary>
public class User
{
    /// <summary>
    ///     User nickname
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    ///     Date and time user registered since
    /// </summary>
    public DateTime RegisteredSince { get; set; }

    /// <summary>
    ///     Date and time user is banned till
    /// </summary>
    public DateTime? BannedTill { get; set; }
    
    /// <summary>
    ///     Additional user information
    /// </summary>
    public UserInformation UserInfo { get; set; }
}
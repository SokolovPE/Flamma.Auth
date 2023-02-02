namespace Flamma.Auth.Models;

/// <summary>
///     User ban information
/// </summary>
public class UserBanInfo
{
    /// <summary>
    ///     User identifier
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    ///     Is user banned
    /// </summary>
    public bool IsBanned { get; set; }

    /// <summary>
    ///     Date and time user is banned till
    /// </summary>
    public DateTime? BannedTill { get; set; }
}
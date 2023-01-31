using AzisFood.DataEngine.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace Flamma.Auth.Data.Access.Models;

/// <summary>
///     User registration request
/// </summary>
[Index(nameof(Username), IsUnique = true)]
public class UserData : PgRepoEntity<AuthDbContext>
{
    /// <summary>
    ///     User login
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    ///     User password
    /// </summary>
    public string PasswordHash { get; set; }
    
    /// <summary>
    ///     User salt
    /// </summary>
    public byte[] Salt { get; set; }
    
    /// <summary>
    ///     Additional user information link
    /// </summary>
    public virtual AdditionalUserInformation AdditionalUserInformation { get; set; }
    
    /// <summary>
    ///     Refresh token of user
    /// </summary>
    public string RefreshToken { get; set; }
    
    /// <summary>
    ///     Lifetime of refresh token
    /// </summary>
    public DateTime RefreshTokenExpiryTime { get; set; }

    /// <summary>
    ///     Date user is banned till
    /// </summary>
    public DateTime? BannedTill { get; set; } = null;
}
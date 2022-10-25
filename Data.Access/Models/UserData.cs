namespace Flamma.Auth.Data.Access.Models;

/// <summary>
///     User registration request
/// </summary>
public class UserData
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
    ///     Additional user information
    /// </summary>
    public AdditionalUserInformation AdditionalUserInformation { get; set; }
}
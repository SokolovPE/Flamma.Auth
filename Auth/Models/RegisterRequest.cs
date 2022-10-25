namespace Flamma.Auth.Models;

/// <summary>
///     User registration request
/// </summary>
public class RegisterRequest
{
    /// <summary>
    ///     User login
    /// </summary>
    public string Username { get; set; }
    
    /// <summary>
    ///     User password
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    ///     Additional user information
    /// </summary>
    public UserInformation UserInformation { get; set; }
}
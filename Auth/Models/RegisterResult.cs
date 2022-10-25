namespace Flamma.Auth.Models;

/// <summary>
///     Represents result of user account registration process
/// </summary>
public class RegisterResult
{
    /// <summary>
    ///     Indicates was registration successful or not
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    ///     Optional message
    /// </summary>
    /// <remarks>Usually filled when process failed</remarks>
    public string Message { get; set; }
}
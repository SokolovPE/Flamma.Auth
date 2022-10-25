namespace Flamma.Auth.Interfaces;

/// <summary>
///     Service to work with dates
/// </summary>
public interface IDateProvider
{
    /// <summary>
    ///     Get current date and time
    /// </summary>
    public DateTime Now { get; }
    
    /// <summary>
    ///     Get current universal date and time
    /// </summary>
    public DateTime UtcNow { get; }
}
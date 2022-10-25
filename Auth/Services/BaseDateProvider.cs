using Flamma.Auth.Interfaces;

namespace Flamma.Auth.Services;

/// <summary>
///     Basic service to work with dates
/// </summary>
public class BaseDateProvider : IDateProvider
{
    /// <inheritdoc />
    public DateTime Now => DateTime.Now;
    
    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}
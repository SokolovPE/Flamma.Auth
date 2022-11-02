using Flamma.Auth.Common.Interfaces;

namespace Flamma.Auth.Common.Services;

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
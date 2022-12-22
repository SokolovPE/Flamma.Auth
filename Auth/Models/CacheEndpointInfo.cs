namespace Flamma.Auth.Models;

/// <summary>
///     Information about cache endpoint
/// </summary>
public class CacheEndpointInfo
{
    /// <summary>
    ///     Endpoint alias
    /// </summary>
    public string Alias { get; set; }
    
    /// <summary>
    ///     Endpoint url
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     Endpoint url
    /// </summary>
    public int Port { get; set; }
}
using Flamma.Auth.Models;
using FluentValidation.Results;

namespace Flamma.Auth.Interfaces;

/// <summary>
///     Operates user accounts
/// </summary>
public interface IAccountManager
{
    /// <summary>
    ///     Validate incoming registration request
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Validation result</returns>
    public Task<ValidationResult> ValidateRegistrationRequestAsync(Models.RegisterRequest request,
        CancellationToken token = default);

    /// <summary>
    ///     Register new user
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="token">Cancellation token</param>
    /// <returns>Result of registration process</returns>
    public Task<RegisterResult> RegisterUserAsync(Models.RegisterRequest request, CancellationToken token = default);

    /// <summary>
    ///     Log user in
    /// </summary>
    public Task<LoginResult> HandleLoginAsync(string username, string password, CancellationToken token = default);

    /// <summary>
    ///     Refresh user access token
    /// </summary>
    public Task<LoginResult> RefreshTokenAsync(string userToken, string userRefreshToken,
        CancellationToken token = default);
    
    /// <summary>
    ///     Revoke user token
    /// </summary>
    Task RevokeTokenAsync(string username, CancellationToken token = default);
    
    /// <summary>
    ///     Revoke all user tokens
    /// </summary>
    Task RevokeAllTokensAsync(CancellationToken token = default);

    /// <summary>
    ///     Ban user for given period
    /// </summary>
    Task BanUserAsync(Guid userId, TimeSpan banPeriod, CancellationToken token = default);

    /// <summary>
    ///     Ban user permanently
    /// </summary>
    Task BanUserAsync(Guid userId, CancellationToken token = default);

    /// <summary>
    ///     Unban user
    /// </summary>
    Task UnbanUserAsync(Guid userId, CancellationToken token = default);
}
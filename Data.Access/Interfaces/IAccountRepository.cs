using Flamma.Auth.Data.Access.Models;

namespace Flamma.Auth.Data.Access.Interfaces;

/// <summary>
///     Manages account data
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    ///     Create new user
    /// </summary>
    public Task CreateUserAsync(UserData userData, CancellationToken token = default);

    /// <summary>
    ///     Check uniqueness of username
    /// </summary>
    public Task<bool> IsUsernameUniqueAsync(string username, CancellationToken token = default);

    /// <summary>
    ///     Validate user over database
    /// </summary>
    Task<bool> ValidateUser(string username, string passwordHash, CancellationToken token = default);

    /// <summary>
    ///     Update refresh token information for given username
    /// </summary>
    Task UpdateUserRefreshToken(string username, string refreshToken, DateTime refreshTokenValidTo,
        CancellationToken token = default);

    /// <summary>
    ///     Get refresh token for given username
    /// </summary>
    Task<string> GetUserRefreshToken(string username, CancellationToken token = default);

    /// <summary>
    ///     Get salt for given username
    /// </summary>
    Task<byte[]> GetUserSalt(string username, CancellationToken token = default);
}
using System.Linq.Expressions;
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
    ///     Update user data
    /// </summary>
    Task UpdateUserAsync(Guid id, UserData userData, CancellationToken token);

    /// <summary>
    ///     Check uniqueness of username
    /// </summary>
    public Task<bool> IsUsernameUniqueAsync(string username, CancellationToken token = default);

    /// <summary>
    ///     Validate user over database
    /// </summary>
    Task<bool> ValidateUserAsync(string username, string passwordHash, CancellationToken token = default);

    /// <summary>
    ///     Update refresh token information for given username
    /// </summary>
    Task UpdateUserRefreshTokenAsync(string username, string refreshToken, DateTime refreshTokenValidTo,
        CancellationToken token = default);

    /// <summary>
    ///     Get refresh token for given username
    /// </summary>
    Task<string> GetUserRefreshTokenAsync(string username, CancellationToken token = default);

    /// <summary>
    ///     Get salt for given username
    /// </summary>
    Task<byte[]> GetUserSaltAsync(string username, CancellationToken token = default);

    /// <summary>
    ///     Get user data by given username
    /// </summary>
    Task<UserData> GetUserDataAsync(string username, CancellationToken token = default);

    /// <summary>
    ///     Get users by given filter
    /// </summary>
    Task<IEnumerable<UserData>> GetUsersAsync(Expression<Func<UserData, bool>> filter = null,
        CancellationToken token = default);

    /// <summary>
    ///     Get user by given id
    /// </summary>
    Task<UserData> GetUserAsync(Guid id, CancellationToken token = default);
}
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
}
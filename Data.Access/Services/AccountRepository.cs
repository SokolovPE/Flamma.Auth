using System.Linq.Expressions;
using AzisFood.DataEngine.Abstractions.Interfaces;
using Flamma.Auth.Data.Access.Interfaces;
using Flamma.Auth.Data.Access.Models;

namespace Flamma.Auth.Data.Access.Services;

/// <summary>
///     Repository that manipulates account data
/// </summary>
public class AccountRepository : IAccountRepository
{
    private readonly IBaseRepository<UserData> _userDataRepository;

    /// <summary>
    ///     .ctor
    /// </summary>
    public AccountRepository(IBaseRepository<UserData> userDataRepository)
    {
        _userDataRepository = userDataRepository;
    }

    /// <inheritdoc />
    public async Task CreateUserAsync(UserData userData, CancellationToken token = default) =>
        await _userDataRepository.CreateAsync(userData, token);

    /// <inheritdoc />
    public async Task UpdateUserAsync(Guid id, UserData userData, CancellationToken token) =>
        await _userDataRepository.UpdateAsync(id, userData, token);

    /// <inheritdoc />
    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken token = default)
    {
        var record =
            await _userDataRepository.ExistsAsync(filter: data => data.Username == username, token: token);
        return !record;
    }
    
    /// <inheritdoc />
    public async Task<bool> ValidateUserAsync(string username, string passwordHash, CancellationToken token = default)
    {
        var userValid = await _userDataRepository.ExistsAsync(
            filter: userData => userData.Username == username && userData.PasswordHash == passwordHash, token);
        return userValid;
    }

    /// <inheritdoc />
    public async Task<byte[]> GetUserSaltAsync(string username, CancellationToken token = default)
    {
        var user = await _userDataRepository.GetAsync(filter: userData => userData.Username == username, track: false,
            token);
        var item = user.FirstOrDefault();
        if (item is null)
        {
            throw new InvalidOperationException($"User with username {username} was not found");
        }

        return item.Salt;
    }

    /// <inheritdoc />
    public async Task UpdateUserRefreshTokenAsync(string username, string refreshToken, DateTime refreshTokenValidTo,
        CancellationToken token = default)
    {
        var request =
            await _userDataRepository.GetAsync(filter: item => item.Username == username, track: false, token: token);
        var item = request.FirstOrDefault();
        if (item is null)
        {
            throw new InvalidOperationException($"User with username {username} was not found");
        }

        item.RefreshTokenExpiryTime = refreshTokenValidTo;
        item.RefreshToken = refreshToken;
        await _userDataRepository.UpdateAsync(item.Id, item, token);
    }

    /// <inheritdoc />
    public async Task<string> GetUserRefreshTokenAsync(string username, CancellationToken token = default)
    {
        var request =
            await _userDataRepository.GetAsync(filter: item => item.Username == username, track: false, token: token);
        var item = request.FirstOrDefault();
        if (item is null)
        {
            throw new InvalidOperationException($"User with username {username} was not found");
        }

        return item.RefreshToken;
    }

    /// <inheritdoc />
    public async Task<UserData> GetUserDataAsync(string username, CancellationToken token = default)
    {
        var request =
            await _userDataRepository.GetAsync(filter: item => item.Username == username, track: false, token: token);
        var item = request.FirstOrDefault();
        if (item is null)
        {
            throw new InvalidOperationException($"User with username {username} was not found");
        }

        return item;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserData>> GetUsersAsync(Expression<Func<UserData, bool>> filter = null,
        CancellationToken token = default) => await _userDataRepository.GetAsync(filter, true, token);

    /// <inheritdoc />
    public async Task<UserData> GetUserAsync(Guid id, CancellationToken token = default)
    {
        var data = await _userDataRepository.GetAsync(id, token: token);
        if(data == null)
            throw new InvalidOperationException($"User with id {id} was not found");
        return data;
    }
}
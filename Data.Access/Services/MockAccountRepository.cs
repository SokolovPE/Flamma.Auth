using System.Linq.Expressions;
using Flamma.Auth.Data.Access.Interfaces;
using Flamma.Auth.Data.Access.Models;

namespace Flamma.Auth.Data.Access.Services;

/// <summary>
///     Mocked repository for development
/// </summary>
public class MockAccountRepository : IAccountRepository
{
    /// <summary>
    ///     Storage imitation
    /// </summary>
    /// <remarks>Key imitates unique index on database table</remarks>
    private readonly Dictionary<string, UserData> _userData;

    /// <summary>
    ///     .ctor
    /// </summary>
    public MockAccountRepository()
    {
        _userData = new Dictionary<string, UserData>
        {
            // Dummy record 1
            {
                "bob", new UserData
                {
                    Id = Guid.Parse("47b70120-b644-406a-8d90-c09bf734737f"),
                    Username = "bob",
                    PasswordHash = "",
                    AdditionalUserInformation = new AdditionalUserInformation
                    {
                        FirstName = "Bob",
                        LastName = "Valentine",
                        PrimaryLocationId = Guid.Parse("d01eb040-e06b-4ed0-99d8-e134e0a061f7"),
                        BirthDate = new DateTime(1990, 5, 14).ToUniversalTime()
                    },
                    BannedTill = new DateTime(2199, 1,1)
                }
            },
            // Dummy record 2
            {
                "rick", new UserData
                {
                    Id = Guid.Parse("1e6501e1-3d08-409b-931e-950a4fd8e902"),
                    Username = "rick",
                    PasswordHash = "",
                    AdditionalUserInformation = new AdditionalUserInformation
                    {
                        FirstName = "Rick",
                        LastName = "Holmes",
                        PrimaryLocationId = Guid.Parse("9e44fe80-328a-46d0-a532-0302caf40c26"),
                        BirthDate = new DateTime(1985, 3, 8).ToUniversalTime()
                    }
                }
            }
        };
    }

    /// <inheritdoc />
    public Task CreateUserAsync(UserData userData, CancellationToken token = default)
    {
        _userData.Add(userData.Username, userData);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UpdateUserAsync(Guid id, UserData userData, CancellationToken token)
    {
        if (!_userData.ContainsKey(userData.Username)) return Task.CompletedTask;
        _userData[userData.Username] = userData;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<bool> IsUsernameUniqueAsync(string username, CancellationToken token = default) =>
        Task.FromResult(!_userData.ContainsKey(username));

    /// <inheritdoc />
    public Task<bool> ValidateUserAsync(string username, string passwordHash, CancellationToken token = default) =>
        Task.FromResult(_userData.Any(userData =>
            userData.Value.Username == username && userData.Value.PasswordHash == passwordHash));

    /// <inheritdoc />
    public Task UpdateUserRefreshTokenAsync(string username, string refreshToken, DateTime refreshTokenValidTo,
        CancellationToken token = default)
    {
        if (!_userData.ContainsKey(username))
        {
            throw new InvalidOperationException($"User with username {username} was not found");
        }
        
        var item = _userData[username];
        item.RefreshTokenExpiryTime = refreshTokenValidTo;
        item.RefreshToken = refreshToken;
        _userData[username] = item;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<string> GetUserRefreshTokenAsync(string username, CancellationToken token = default)
    {
        if (!_userData.ContainsKey(username))
        {
            throw new InvalidOperationException($"User with username {username} was not found");
        }
        
        var item = _userData[username];
        return Task.FromResult(item.RefreshToken);
    }

    /// <inheritdoc />
    public Task<byte[]> GetUserSaltAsync(string username, CancellationToken token = default)
    {
        if (!_userData.ContainsKey(username))
        {
            throw new InvalidOperationException($"User with username {username} was not found");
        }

        return Task.FromResult(_userData[username].Salt);
    }

    /// <inheritdoc />
    public Task<UserData> GetUserDataAsync(string username, CancellationToken token = default)
    {
        if (!_userData.ContainsKey(username))
        {
            throw new InvalidOperationException($"User with username {username} was not found");
        }
        
        var item = _userData[username];
        return Task.FromResult(item);
    }

    /// <inheritdoc />
    public Task<IEnumerable<UserData>> GetUsersAsync(Expression<Func<UserData, bool>> filter = null, CancellationToken token = default)
    {
        var query = _userData.Select(x => x.Value);
        if (filter != null)
            query = query.Where(filter.Compile());

        var data = query.ToList();
        return Task.FromResult<IEnumerable<UserData>>(data);
    }

    /// <inheritdoc />
    public Task<UserData> GetUserAsync(Guid id, CancellationToken token = default)
    {
        var data = _userData.FirstOrDefault(data => data.Value.Id == id);
        return Task.FromResult(data.Value);
    }

    /// <inheritdoc />
    public async Task<DateTime?> GetUserBanDateAsync(Guid id, CancellationToken token = default)
    {
        var bannedTill = _userData.Where(data => data.Value.Id == id)
            .Select(userData => userData.Value.BannedTill)
            .FirstOrDefault();
        return bannedTill;
    }
}
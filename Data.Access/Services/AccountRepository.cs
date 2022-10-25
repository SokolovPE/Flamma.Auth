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
    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken token = default)
    {
        var record =
            await _userDataRepository.GetAsync(filter: data => data.Username == username, track: false, token: token);
        return !record.Any();
    }
}
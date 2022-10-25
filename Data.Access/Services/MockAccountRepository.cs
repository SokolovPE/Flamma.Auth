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
                "bobv", new UserData
                {
                    Username = "bobv",
                    PasswordHash = "",
                    AdditionalUserInformation = new AdditionalUserInformation
                    {
                        FirstName = "Bob",
                        LastName = "Valentine",
                        PrimaryLocationId = Guid.Parse("d01eb040-e06b-4ed0-99d8-e134e0a061f7"),
                        BirthDate = new DateTime(1990, 5, 14)
                    }
                }
            },
            // Dummy record 2
            {
                "rick", new UserData
                {
                    Username = "rick",
                    PasswordHash = "",
                    AdditionalUserInformation = new AdditionalUserInformation
                    {
                        FirstName = "Rick",
                        LastName = "Holmes",
                        PrimaryLocationId = Guid.Parse("9e44fe80-328a-46d0-a532-0302caf40c26"),
                        BirthDate = new DateTime(1985, 3, 8)
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
#pragma warning disable CS1998
    public async Task<bool> IsUsernameUniqueAsync(string username, CancellationToken token = default) =>
#pragma warning restore CS1998
        !_userData.ContainsKey(username);
}
using Flamma.Auth.Data.Access.Models;
using Microsoft.Extensions.Logging;

namespace Flamma.Auth.Data.Access;

/// <summary>
///     This seeder fills auth database with initial values
///     Can be disabled in application settings
/// </summary>
public class AuthDbSeeder
{
    /// <summary>
    ///     Primary auth database context
    /// </summary>
    private readonly AuthDbContext _context;

    /// <summary>
    ///     Logger
    /// </summary>
    private readonly ILogger<AuthDbSeeder> _logger;

    /// <summary>
    ///     .ctor
    /// </summary>
    public AuthDbSeeder(AuthDbContext context, ILogger<AuthDbSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    ///     Fills database with initial values
    /// </summary>
    public void Seed()
    {
        SeedUsers();
    }

    private void SeedUsers()
    {
        if(_context.UserData.Any())
            return;
        
        var userSeed = new []
        {
            // Dummy record 1
            new UserData
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
            },
            // Dummy record 2
            new UserData
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
        };
        
        _context.UserData.AddRange(userSeed);
        var changes = _context.SaveChanges();
        if (changes != userSeed.Length)
            _logger.LogWarning(
                "[UserData] Seed length {UserDataSeedLength} is not equal to added row count: {Changes}",
                userSeed.Length, changes);
    }
}
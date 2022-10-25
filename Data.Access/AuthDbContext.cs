using AzisFood.DataEngine.Core.Attributes;
using Microsoft.EntityFrameworkCore;

namespace Flamma.Auth.Data.Access;

/// <summary>
///     Primary database context
/// </summary>
[ConnectionAlias("auth")]
public class AuthDbContext : DbContext
{
    /// <summary>
    ///     .ctor
    /// </summary>
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    /// <summary>
    ///     Set of additional user information
    /// </summary>
    public DbSet<Models.AdditionalUserInformation> AdditionalUserInformation { get; set; } = null!;

    /// <summary>
    ///     Set of user data
    /// </summary>
    public DbSet<Models.UserData> UserData { get; set; } = null!;
}
using Flamma.Auth.Data.Access.Interfaces;
using Flamma.Auth.Data.Access.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Flamma.Auth.Data.Access.Extensions;

/// <summary>
///     Dependency Injection helpers
/// </summary>
public static class DiExtensions
{
    /// <summary>
    ///     Add data access containers
    /// </summary>
    public static IServiceCollection AddDataAccess(this IServiceCollection serviceCollection)
    {
        // Add services
        serviceCollection.AddSingleton<IAccountRepository, AccountRepository>();
        // serviceCollection.AddSingleton<IAccountRepository, MockAccountRepository>();

        // serviceCollection.AddScoped<AuthDbSeeder>();
        
        return serviceCollection;
    }
    
    /// <summary>
    ///     Add initial filler of database
    /// </summary>
    /// <param name="serviceProvider">Provider of services</param>
    /// <param name="configuration">Config of application</param>
    public static void AddSeeder(this IServiceProvider serviceProvider, IConfiguration configuration)
    {
        // Read configuration, if seeder disabled - nothing to do here
        var seederEnabled = configuration.GetSection("EnableSeeder").Get<bool>();
        if(!seederEnabled) return;
        
        // Get scope factory, if failed - nothing to do here
        var scopedFactory = serviceProvider.GetService<IServiceScopeFactory>();
        if (scopedFactory == null) return;
        
        // Create scope and seed initial data
        using var scope = scopedFactory.CreateScope();
        var seeder = serviceProvider.GetRequiredService<AuthDbSeeder>();
        seeder.Seed();
    }
}
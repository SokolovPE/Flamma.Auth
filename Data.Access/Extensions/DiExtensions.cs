using Flamma.Auth.Data.Access.Interfaces;
using Flamma.Auth.Data.Access.Services;
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
        // serviceCollection.AddScoped<IAccountRepository, ...>();
        serviceCollection.AddSingleton<IAccountRepository, MockAccountRepository>();
        
        return serviceCollection;
    }
}
using Flamma.Auth.Data.Access.Extensions;
using Flamma.Auth.Interfaces;
using Flamma.Auth.Services;
using Flamma.Auth.Validators;
using FluentValidation;

namespace Flamma.Auth.Extensions;

/// <summary>
///     Dependency Injection helpers
/// </summary>
public static class DiExtensions
{
    /// <summary>
    ///     Add core service containers
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection serviceCollection)
    {
        // Add services
        serviceCollection.AddDataAccess();
        
        serviceCollection.AddScoped<IAccountManager, Services.CoreAccountManager>();
        serviceCollection.AddSingleton<IDateProvider, Services.BaseDateProvider>();
        serviceCollection.AddSingleton<IHasher, Services.Hasher>();
        
        // Add validators
        serviceCollection.AddScoped<IValidator<Models.RegisterRequest>, RegisterRequestValidator>();
        
        return serviceCollection;
    }
}
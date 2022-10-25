using Flamma.Auth.Models;
using FluentValidation.Results;

namespace Flamma.Auth.Interfaces;

/// <summary>
///     Operates user accounts
/// </summary>
public interface IAccountManager
{
    /// <summary>
    ///     Validate incoming registration request
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <returns>Validation result</returns>
    public Task<ValidationResult> ValidateRegistrationRequestAsync(Models.RegisterRequest request, CancellationToken token = default);
    
    /// <summary>
    ///     Register new user
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <returns>Result of registration process</returns>
    public Task<RegisterResult> RegisterUserAsync(Models.RegisterRequest  request, CancellationToken token = default);
}
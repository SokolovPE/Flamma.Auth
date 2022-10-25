using AutoMapper;
using Flamma.Auth.Data.Access.Interfaces;
using Flamma.Auth.Interfaces;
using Flamma.Auth.Models;
using FluentValidation;
using FluentValidation.Results;

namespace Flamma.Auth.Services;

/// <summary>
///     Basic account manager
/// </summary>
public class CoreAccountManager : IAccountManager
{
    /// <summary>
    ///     Logger
    /// </summary>
    /// <remarks>Protected for possible decorators</remarks>
    // ReSharper disable once MemberCanBePrivate.Global
    protected readonly ILogger<CoreAccountManager> Logger;

    /// <summary>
    ///     AutoMapper
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     Registration request validator
    /// </summary>
    private readonly IValidator<Models.RegisterRequest> _requestValidator;

    /// <summary>
    ///     Hash calculation service
    /// </summary>
    private readonly IHasher _hasher;

    /// <summary>
    ///     Account repository
    /// </summary>
    private readonly IAccountRepository _accountRepository;

    /// <summary>
    ///     .ctor
    /// </summary>
    public CoreAccountManager(ILogger<CoreAccountManager> logger, IValidator<Models.RegisterRequest> requestValidator,
        IHasher hasher, IAccountRepository accountRepository, IMapper mapper)
    {
        Logger = logger;
        _requestValidator = requestValidator;
        _hasher = hasher;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateRegistrationRequestAsync(Models.RegisterRequest request,
        CancellationToken token = default)
    {
        try
        {
            var result = await _requestValidator.ValidateAsync(request, token);

            if (!result.IsValid)
            {
                Logger.LogWarning("Registration request {@RegistrationRequest} validation failed: {@ValidationResult}",
                    request, result);
            }

            return result;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error during validation of registration request {@RegistrationRequest}", request);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<RegisterResult> RegisterUserAsync(Models.RegisterRequest request,
        CancellationToken token = default)
    {
        try
        {
            // Calculate password hash
            var passwordHash = _hasher.HashString(request.Password);

            // Imitation of Data.Access activity
            // TODO: Here Data.Access storage service must be called
            var registrationData = _mapper.Map<Data.Access.Models.UserData>(request);
            registrationData.PasswordHash = passwordHash;
            
            await _accountRepository.CreateUserAsync(registrationData, token);

            return new RegisterResult
            {
                Success = true,
                Message = string.Empty
            };

        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error during user registration, request: {@RegistrationRequest}", request);
            return new RegisterResult
            {
                Success = false,
                Message = e.Message
            };
        }
    }
}
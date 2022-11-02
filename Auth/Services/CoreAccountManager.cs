using AutoMapper;
using Flamma.Auth.Common.Interfaces;
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
    ///     Token generator
    /// </summary>
    private readonly IJwtGenerator _jwtGenerator;

    /// <summary>
    ///     .ctor
    /// </summary>
    public CoreAccountManager(ILogger<CoreAccountManager> logger, IValidator<Models.RegisterRequest> requestValidator,
        IHasher hasher, IAccountRepository accountRepository, IMapper mapper, IJwtGenerator jwtGenerator)
    {
        Logger = logger;
        _requestValidator = requestValidator;
        _hasher = hasher;
        _accountRepository = accountRepository;
        _mapper = mapper;
        _jwtGenerator = jwtGenerator;
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
            var salt = _hasher.MakeSalt();
            var passwordHash = _hasher.HashString(request.Password, salt);
            
            var registrationData = _mapper.Map<Data.Access.Models.UserData>(request);
            registrationData.PasswordHash = passwordHash;
            registrationData.Salt = salt;
            
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

    /// <inheritdoc />
    public async Task<LoginResult> HandleLoginAsync(string username, string password, CancellationToken token = default)
    {
        try
        {
            // Calculate password hash
            var userSalt = await _accountRepository.GetUserSalt(username, token);
            var passwordHash = _hasher.HashString(password, userSalt);

            var isUserValid = await _accountRepository.ValidateUser(username, passwordHash, token);
            if (!isUserValid)
            {
                throw new InvalidOperationException($"User validation failed for {username}");
            }
            
            // TODO: Get Roles here and create claims, for now no need
            var jwtTokenInfo = _jwtGenerator.GenerateToken(username);

            return new LoginResult
            {
                Success = true,
                TokenInfo = jwtTokenInfo
            };
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error during user login, user: {Username}", username);
            return new LoginResult
            {
                Success = false
            };
        }
    }
}
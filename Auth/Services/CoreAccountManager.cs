using AutoMapper;
using AzisFood.CacheService.Redis.Interfaces;
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
    ///     Cache service
    ///     TODO: Replace with another cache package, in airplane have no wi-fi to use another
    /// </summary>
    private readonly IRedisCacheService _cacheService;

    /// <summary>
    ///     Registration request validator
    /// </summary>
    private readonly IValidator<Models.RegisterRequest> _registrationRequestValidator;

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
    private readonly IJwtGenerator _jwtManager;

    /// <summary>
    ///     .ctor
    /// </summary>
    public CoreAccountManager(ILogger<CoreAccountManager> logger,
        IValidator<Models.RegisterRequest> registrationRequestValidator,
        IHasher hasher, IAccountRepository accountRepository, IMapper mapper, IJwtGenerator jwtManager,
        IRedisCacheService cacheService)
    {
        Logger = logger;
        _registrationRequestValidator = registrationRequestValidator;
        _hasher = hasher;
        _accountRepository = accountRepository;
        _mapper = mapper;
        _jwtManager = jwtManager;
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateRegistrationRequestAsync(Models.RegisterRequest request,
        CancellationToken token = default)
    {
        try
        {
            var result = await _registrationRequestValidator.ValidateAsync(request, token);

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
            var jwtTokenInfo = _jwtManager.GenerateToken(username);

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
    
    /// <inheritdoc />
    public async Task<bool> ValidateTokenAsync(string userToken, string username, CancellationToken token = default)
    {
        try
        {
            // Check in cache is token presented, if it is no need to validate
            var cachedToken = await _cacheService.GetAsync<string>($"user_token:{username}");
            if (!string.IsNullOrWhiteSpace(cachedToken))
                return true;
            
            // If it's not - validate it
            var tokenValidationResult =  _jwtManager.ValidateToken(userToken, username);

            switch (tokenValidationResult)
            {
                // If token is valid - store in cache
                case JwtTokenStatus.Valid:
                    await _cacheService.SetAsync($"user_token:{username}", userToken,
                        TimeSpan.FromSeconds(_jwtManager.GetTokenValidityCheckPeriod()));
                    break;
                // If token is expired - refresh it
                case JwtTokenStatus.Expired:
                {
                    var refreshToken = await _accountRepository.GetUserRefreshToken(username, token);
                    var refreshedToken = _jwtManager.RefreshToken(userToken, refreshToken);
                    return await ValidateTokenAsync(refreshedToken.Token, username, token);
                }
                case JwtTokenStatus.Invalid:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Current token validation status is not supported yet, status: {tokenValidationResult}");
            }

            return true;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error during token validation, user: {Username}, token: {Token}", username, userToken);
            throw;
        }
    }
}
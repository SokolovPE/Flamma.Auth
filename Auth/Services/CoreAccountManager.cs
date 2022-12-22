using AutoMapper;
using EasyCaching.Core;
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
    ///     Main cache alias for accounts
    /// </summary>
    private const string CacheAlias = "Redis";
    
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
    /// </summary>
    private readonly IEasyCachingProviderFactory _cacheFactory;

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
    private readonly IJwtManager _jwtManager;

    /// <summary>
    ///     Application configuration
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    ///     Date provider
    /// </summary>
    private readonly IDateProvider _dateProvider;

    /// <summary>
    ///     .ctor
    /// </summary>
    public CoreAccountManager(ILogger<CoreAccountManager> logger,
        IValidator<Models.RegisterRequest> registrationRequestValidator,
        IHasher hasher, IAccountRepository accountRepository, IMapper mapper, IJwtManager jwtManager,
        IEasyCachingProviderFactory cacheFactory, IConfiguration configuration, IDateProvider dateProvider)
    {
        Logger = logger;
        _registrationRequestValidator = registrationRequestValidator;
        _hasher = hasher;
        _accountRepository = accountRepository;
        _mapper = mapper;
        _jwtManager = jwtManager;
        _cacheFactory = cacheFactory;
        _configuration = configuration;
        _dateProvider = dateProvider;
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
            var userSalt = await _accountRepository.GetUserSaltAsync(username, token);
            var passwordHash = _hasher.HashString(password, userSalt);

            var isUserValid = await _accountRepository.ValidateUserAsync(username, passwordHash, token);
            if (!isUserValid)
            {
                throw new InvalidOperationException($"User validation failed for {username}");
            }

            // TODO: Get Roles here and create claims, for now no need
            var jwtTokenInfo = _jwtManager.GenerateToken(username);
            
            _ = int.TryParse(_configuration["Jwt:RefreshTokenValidityInDays"], out var refreshTokenValidityInDays);
            await _accountRepository.UpdateUserRefreshTokenAsync(username, jwtTokenInfo.RefreshToken,
                _dateProvider.UtcNow.AddDays(refreshTokenValidityInDays), token);

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
    public async Task<LoginResult> RefreshTokenAsync(string userToken, string userRefreshToken,
        CancellationToken token = default)
    {
        try
        {
            var username = _jwtManager.ExtractUsername(userToken);
            var userData = await _accountRepository.GetUserDataAsync(username, token);
            if (userData == default || userData.RefreshToken != userRefreshToken ||
                userData.RefreshTokenExpiryTime <= _dateProvider.UtcNow)
                throw new ArgumentException("Invalid access token or refresh token");

            var refreshedTokenInfo = _jwtManager.RefreshToken(userToken);
            _ = int.TryParse(_configuration["Jwt:RefreshTokenValidityInDays"], out var refreshTokenValidityInDays);
            await _accountRepository.UpdateUserRefreshTokenAsync(username, refreshedTokenInfo.RefreshToken,
                _dateProvider.UtcNow.AddDays(refreshTokenValidityInDays), token);
            return new LoginResult
            {
                Success = true,
                TokenInfo = refreshedTokenInfo
            };
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error during refresh token: {Token} using refresh token: {RefreshToken}", userToken,
                userRefreshToken);
            return new LoginResult
            {
                Success = false
            };
        }
    }
}
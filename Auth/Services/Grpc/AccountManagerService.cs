using AutoMapper;
using Flamma.Auth.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Flamma.Auth.Services;

/// <summary>
///     Service to manage user account
/// </summary>
public class AccountManagerService : AccountManager.AccountManagerBase
{
    /// <summary>
    ///     Logger service
    /// </summary>
    private readonly ILogger<AccountManagerService> _logger;

    /// <summary>
    ///     AutoMapper
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     Account manager service
    /// </summary>
    private readonly IAccountManager _accountManager;

    /// <summary>
    ///     .ctor
    /// </summary>
    public AccountManagerService(ILogger<AccountManagerService> logger, IAccountManager accountManager, IMapper mapper)
    {
        _logger = logger;
        _accountManager = accountManager;
        _mapper = mapper;
    }

    /// <summary>
    ///     Register new user
    /// </summary>
    public override async Task<RegisterReply> Register(RegisterRequest request, ServerCallContext context)
    {
        _logger.LogInformation("New account registration request: {@RegistrationRequest}", request);
        var mappedRequest = _mapper.Map<Models.RegisterRequest>(request);

        // Validation
        var validationResult =
            await _accountManager.ValidateRegistrationRequestAsync(mappedRequest, context.CancellationToken);
        if (!validationResult.IsValid)
        {
            return new RegisterReply
            {
                Status = CommonStatus.Invalid,
                Message = validationResult.ToString()
            };
        }

        // Try to register user
        var result = await _accountManager.RegisterUserAsync(mappedRequest, context.CancellationToken);
        var response = _mapper.Map<RegisterReply>(result);
        return response;
    }

    /// <summary>
    ///     Log user in
    /// </summary>
    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        _logger.LogInformation("New user login request: {@LoginRequest}", request);
        try
        {
            var handleResult =
                await _accountManager.HandleLoginAsync(request.Username, request.Password, context.CancellationToken);
            var result = _mapper.Map<LoginResponse>(handleResult);
            return result;
        }
        catch (InvalidOperationException)
        {
            return new LoginResponse
            {
                Status = CommonStatus.Invalid
            };
        }
    }

    /// <summary>
    ///     Refresh user access token
    /// </summary>
    [Authorize]
    public override async Task<LoginResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
    {
        _logger.LogInformation("New refresh token request: {@RefreshRequest}", request);
        try
        {
            var handleResult =
                await _accountManager.RefreshTokenAsync(request.Token, request.RefreshToken, context.CancellationToken);
            var result = _mapper.Map<LoginResponse>(handleResult);
            return result;
        }
        catch (InvalidOperationException)
        {
            return new LoginResponse
            {
                Status = CommonStatus.Invalid
            };
        }
    }

    /// <inheritdoc />
    [Authorize]
    public override async Task<Empty> RevokeToken(RevokeTokenRequest request, ServerCallContext context)
    {
        _logger.LogInformation("New revoke token request: {@RevokeTokenRequest}", request);
        await _accountManager.RevokeTokenAsync(request.Token, context.CancellationToken);
        return new Empty();
    }

    /// <inheritdoc />
    [Authorize]
    public override async Task<Empty> RevokeAllTokens(Empty request, ServerCallContext context)
    {
        _logger.LogInformation("New revoke all tokens request");
        await _accountManager.RevokeAllTokensAsync(context.CancellationToken);
        return new Empty();
    }

    /// <inheritdoc />
    [Authorize]
    public override async Task<Empty> Ban(BanRequest request, ServerCallContext context)
    {
        _logger.LogInformation("New ban user request: {@BanRequest}", request);
        var idParseSuccessful = Guid.TryParse(request.UserId, out var userId);
        if (!idParseSuccessful)
        {
            _logger.LogWarning("Impossible to parse user id {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"Impossible to parse user id: {request.UserId}"));
        }

        try
        {
            await _accountManager.BanUserAsync(userId, request.BanPeriod.ToDateTimeOffset().TimeOfDay,
                context.CancellationToken);
            return new Empty();
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError(e, "User to ban was not found, Request: {@BanRequest}", request);
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"User with id {request.UserId} was not found"));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception during ban of user, Request: {@BanRequest}", request);
            throw;
        }
    }

    /// <inheritdoc />
    [Authorize]
    public override async Task<Empty> PermanentBan(UserIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation("New permanent ban user request: {@BanRequest}", request);
        var idParseSuccessful = Guid.TryParse(request.UserId, out var userId);
        if (!idParseSuccessful)
        {
            _logger.LogWarning("Impossible to parse user id {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"Impossible to parse user id: {request.UserId}"));
        }

        try
        {
            await _accountManager.BanUserAsync(userId, context.CancellationToken);
            return new Empty();
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError(e, "User to permanently ban was not found, Request: {@BanRequest}", request);
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"User with id {request.UserId} was not found"));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception during permanent ban of user, Request: {@BanRequest}", request);
            throw;
        }
    }

    /// <inheritdoc />
    [Authorize]
    public override async Task<Empty> Unban(UserIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation("New unban user request: {@UnbanRequest}", request);
        var idParseSuccessful = Guid.TryParse(request.UserId, out var userId);
        if (!idParseSuccessful)
        {
            _logger.LogWarning("Impossible to parse user id {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"Impossible to parse user id: {request.UserId}"));
        }

        try
        {
            await _accountManager.UnbanUserAsync(userId, context.CancellationToken);
            return new Empty();
        }
        catch (InvalidOperationException e)
        {
            _logger.LogError(e, "User to unban was not found, Request: {@UnbanRequest}", request);
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"User with id {request.UserId} was not found"));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception during unban of user, Request: {@UnbanRequest}", request);
            throw;
        }
    }

    /// <inheritdoc />
    [Authorize]
    public override async Task<IsBannedResponse> IsBanned(UserIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation("New ban state check request: {@IsBannedRequest}", request);
        var idParseSuccessful = Guid.TryParse(request.UserId, out var userId);
        if (!idParseSuccessful)
        {
            _logger.LogWarning("Impossible to parse user id {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                $"Impossible to parse user id: {request.UserId}"));
        }
        try
        {
            var info = await _accountManager.IsUserBannedAsync(userId, context.CancellationToken);
            return _mapper.Map<IsBannedResponse>(info);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unhandled exception during ban state check of user, Request: {@IsBannedRequest}",
                request);
            throw;
        }
    }

    /// <inheritdoc />
    [Authorize]
    public override async Task<UserInfoResponse> GetUserById(UserIdRequest request, ServerCallContext context)
    {
        return await base.GetUserById(request, context);
    }

    /// <inheritdoc />
    [Authorize]
    public override async Task<UserInfoResponse> GetUserByUsername(UserNameRequest request, ServerCallContext context)
    {
        return await base.GetUserByUsername(request, context);
    }

    [Authorize]
    public override async Task<TestResponse> Test(Empty request, ServerCallContext context)
    {
        return new TestResponse
        {
            Message = "Hello world"
        };
    }
}
using AutoMapper;
using Flamma.Auth.Interfaces;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Flamma.Auth.Services;

/// <summary>
///     Service to managet user account
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
    ///     Validate users JWT token
    /// </summary>
    public override async Task<TokenValidationResponse> ValidateToken(TokenValidationRequest request,
        ServerCallContext context)
    {
        try
        {
            var tokenValid =
                await _accountManager.ValidateTokenAsync(request.Token, request.Username, context.CancellationToken);
            return new TokenValidationResponse
            {
                Status = CommonStatus.Success,
                ValidationResult = tokenValid
            };
        }
        catch (Exception)
        {
            return new TokenValidationResponse
            {
                Status = CommonStatus.Fail
            };
        }
    }
}
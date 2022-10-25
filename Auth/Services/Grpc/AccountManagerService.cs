﻿using AutoMapper;
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
        var validationResult = await _accountManager.ValidateRegistrationRequestAsync(mappedRequest);
        if (!validationResult.IsValid)
        {
            return new RegisterReply
            {
                Status = RegistrationStatus.Invalid,
                Message = validationResult.ToString()
            };
        }

        // Try to register user
        var result = await _accountManager.RegisterUserAsync(mappedRequest);
        var response =  _mapper.Map<RegisterReply>(result);
        return response;
    }
}
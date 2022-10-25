using Flamma.Auth.Data.Access.Interfaces;
using FluentValidation;

namespace Flamma.Auth.Validators;

public class RegisterRequestValidator : AbstractValidator<Models.RegisterRequest>
{
    /// <summary>
    ///     Repository that manages accounts
    /// </summary>
    private readonly IAccountRepository _accountRepository;
    
    /// <summary>
    ///     .ctor
    /// </summary>
    public RegisterRequestValidator(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
        
        // Username must contain only digits and letters
        RuleFor(x => x.Username)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9]*$").WithMessage("'{PropertyName}' must contain only digits and letters.")
            .MustAsync(BeNewUsername).WithMessage("Username is already taken by another user.");
        
        // Password must contain digits, letters upper and lowercase, special symbols, at least 6 symbols
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Matches("[a-z]").WithMessage("'{PropertyName}' must contain one or more lowercase letters.")
            .Matches("[A-Z]").WithMessage("'{PropertyName}' must contain one or more capital letters.")
            .Matches(@"[][""!@$%^&*(){}:;<>,.?/+_=|'~\\-]")
            .WithMessage("'{PropertyName}' must contain one or more special characters.")
            .Matches(@"\d").WithMessage("'{PropertyName}' must contain one or more digits.");
        
        // Firstname contain only letters
        RuleFor(x => x.UserInformation.FirstName)
            .NotEmpty()
            .Matches("^[a-zA-Z]*$").WithMessage("'{PropertyName}' must contain only letters.");
        
        // Lastname contain only letters
        RuleFor(x => x.UserInformation.LastName)
            .NotEmpty()
            .Matches("^[a-zA-Z]*$").WithMessage("'{PropertyName}' must contain only letters.");
        
        // Primary location exists
        RuleFor(x => x.UserInformation.PrimaryLocationId)
            .NotEmpty()
            .MustAsync(BeAValidLocation).WithMessage("Please specify a valid primary location");
    }

    /// <summary>
    ///     Main location validator
    /// </summary>
    private async Task<bool> BeAValidLocation(Guid locationId, CancellationToken token = default)
    {
        // For now do not check location
        // When location service will be ready - enable validation
        return true;
    }

    /// <summary>
    ///     Username uniqueness validator
    /// </summary>
    private async Task<bool> BeNewUsername(string username, CancellationToken token = default)
    {
        var result = await _accountRepository.IsUsernameUniqueAsync(username, token);
        return result;
    }
}
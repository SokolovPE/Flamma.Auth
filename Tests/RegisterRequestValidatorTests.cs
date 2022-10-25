using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flamma.Auth.Data.Access.Interfaces;
using Flamma.Auth.Models;
using Flamma.Auth.Validators;
using FluentValidation.TestHelper;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Flamma.Auth.Tests;

public class RegisterRequestValidatorTests : IClassFixture<RegisterFixture>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly RegisterFixture _fixture;
    private readonly RegisterRequestValidator _validator;

    // Mocks
    private readonly Mock<IAccountRepository> _accountRepository;
    public RegisterRequestValidatorTests(ITestOutputHelper testOutputHelper, RegisterFixture fixture)
    {
        // Initialize mock
        _accountRepository = new Mock<IAccountRepository>();
        _testOutputHelper = testOutputHelper;
        _fixture = fixture;

        _validator = new RegisterRequestValidator(_accountRepository.Object);

        // Setup mock
        Setup();
    }

    /// <summary>
    ///     Setup mock
    /// </summary>
    private void Setup()
    {
        // Setup uniqueness of username validator
        _accountRepository.Setup(repo => repo.IsUsernameUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string username, CancellationToken _) => !RegisterFixture.ReservedUsernames.Contains(username));
    }

    [Fact]
    public async Task ValidateRequest_GotNonUniqueUsername_ReturnsInvalidValidationResult()
    {
        var request = _fixture.MakeNonUniqueRegisterRequest();
        var result = await _validator.TestValidateAsync(request);
        result.ShouldHaveValidationErrorFor(registerRequest => registerRequest.Username);
    }
}
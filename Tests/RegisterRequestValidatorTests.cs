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
        // Arrange
        var request = _fixture.MakeNonUniqueRegisterRequest();
        
        // Act
        var result = await _validator.TestValidateAsync(request);
        
        // Assert
        result.ShouldHaveValidationErrorFor(registerRequest => registerRequest.Username);
    }

    [Theory]
    [InlineData("12345", false)]
    [InlineData("qwerty12345", false)]
    [InlineData("Qwerty12345", false)]
    [InlineData("Qwerty12345!", true)]
    public async Task ValidateRequest_ValidatePassword(string password, bool expected)
    {
        // Arrange
        var request = _fixture.MakeNonUniqueRegisterRequest();
        request.Password = password;
        
        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(registerRequest => registerRequest.Password);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(registerRequest => registerRequest.Password);
        }
    }

    [Theory]
    [InlineData("111", false)]
    [InlineData("name1", false)]
    [InlineData("name!", false)]
    [InlineData("name", true)]
    public async Task ValidateRequest_ValidateFirstName(string firstName, bool expected)
    {
        // Arrange
        var request = _fixture.MakeNonUniqueRegisterRequest();
        request.UserInformation.FirstName = firstName;
        
        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(registerRequest => registerRequest.UserInformation.FirstName);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(registerRequest => registerRequest.UserInformation.FirstName);
        }
    }

    [Theory]
    [InlineData("111", false)]
    [InlineData("name1", false)]
    [InlineData("name!", false)]
    [InlineData("name", true)]
    public async Task ValidateRequest_ValidateLastName(string firstName, bool expected)
    {
        // Arrange
        var request = _fixture.MakeNonUniqueRegisterRequest();
        request.UserInformation.LastName = firstName;
        
        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        if (expected)
        {
            result.ShouldNotHaveValidationErrorFor(registerRequest => registerRequest.UserInformation.LastName);
        }
        else
        {
            result.ShouldHaveValidationErrorFor(registerRequest => registerRequest.UserInformation.LastName);
        }
    }
}
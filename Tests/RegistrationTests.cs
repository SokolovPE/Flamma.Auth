// using System.Threading;
// using System.Threading.Tasks;
// using Flamma.Auth.Interfaces;
// using Flamma.Auth.Models;
// using Flamma.Auth.Services;
// using Microsoft.Extensions.Logging;
// using Moq;
// using Xunit;
// using Xunit.Abstractions;
//
// namespace Tests;
//
// public class RegistrationTests
// {
//     private readonly IAccountManager _accountManager;
//     private readonly ITestOutputHelper _testOutputHelper;
//     
//     // Mocks
//
//     public RegistrationTests(ITestOutputHelper testOutputHelper)
//     {
//         _testOutputHelper = testOutputHelper;
//         
//         // Initialize mock
//         
//         // Setup mock
//         Setup();
//         
//         _accountManager = new CoreAccountManager(Mock.Of<ILogger<CoreAccountManager>>(), )
//     }
//
//     /// <summary>
//     ///     Setup mock
//     /// </summary>
//     private void Setup()
//     {
//         
//     }
//
//     [Fact]
//     public async Task ValidateRequest_GotNonUniqueUsername_ReturnsInvalidValidationResult
//     {
//         
//     }
// }
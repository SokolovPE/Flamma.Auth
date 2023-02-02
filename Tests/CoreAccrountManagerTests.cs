// using System.Threading;
// using AutoMapper;
// using Flamma.Auth.Common.Interfaces;
// using Flamma.Auth.Data.Access.Interfaces;
// using Flamma.Auth.Interfaces;
// using Flamma.Auth.Services;
// using FluentValidation;
// using Microsoft.Extensions.Logging;
// using Moq;
//
// namespace Flamma.Auth.Tests;
//
// public class CoreAccrountManagerTests
// {
//     private readonly IAccountManager _accountManager;
//     
//     // Mocks
//     private readonly Mock<IAccountRepository> _accountRepository;
//
//     public CoreAccrountManagerTests()
//     {
//         // Initialize mock
//         _accountRepository = new Mock<IAccountRepository>();
//         
//         var logger = new Mock<ILogger<CoreAccountManager>>();
//         var validator = new Mock<IValidator<Flamma.Auth.Models.RegisterRequest>>();
//         var hasher = new Mock<IHasher>();
//         var mapper = new Mock<IMapper>();
//         _accountManager = new CoreAccountManager(logger.Object, validator.Object, hasher.Object, _accountRepository.Object, mapper.Object, )
//
//         // Setup mock
//         Setup();
//     }
//
//     /// <summary>
//     ///     Setup mock
//     /// </summary>
//     private void Setup()
//     {
//     }
// }
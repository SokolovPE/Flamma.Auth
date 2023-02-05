using System;
using Bogus;
using Flamma.Auth.Models;

namespace Flamma.Auth.Tests;

public class RegisterFixture
{
    private const string Locale = "en_US";
    
    /// <summary>
    ///     Usernames which are reserved in database
    /// </summary>
    public static readonly string[] ReservedUsernames = { "bob", "rick" };

    /// <summary>
    ///     Location identifiers which are reserved in database
    /// </summary>
    public static readonly Guid[] ReservedLocationIds =
        { Guid.Parse("9e44fe80-328a-46d0-a532-0302caf40c26"), Guid.Parse("9d213698-e55f-45ff-88d0-2b773a5a1ef9") };

    /// <summary>
    ///     Fake generator of non-unique account registration request with reserved username
    /// </summary>
    private readonly Faker<Models.RegisterRequest> _nonUniqueRegistrationRequest =
        new Faker<Models.RegisterRequest>(Locale).CustomInstantiator(f => new Models.RegisterRequest
        {
            Username = f.PickRandom(ReservedUsernames),
            // "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]*$" pattern is not working
            Password = f.Internet.Password(),
            UserInformation = new UserInformation
            {
                FirstName = f.Person.FirstName,
                LastName = f.Person.LastName,
                PrimaryLocationId = f.PickRandom(ReservedLocationIds),
                BirthDate = f.Date.Past(24).ToUniversalTime()
            }
        });

    /// <summary>
    ///     Generate one non-unique account registration request (username is reserved)
    /// </summary>
    public Models.RegisterRequest MakeNonUniqueRegisterRequest() => _nonUniqueRegistrationRequest.Generate();
}
using System.Security.Cryptography;
using Flamma.Auth.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Flamma.Auth.Services;

/// <inheritdoc />
public class Hasher : IHasher
{
    /// <inheritdoc />
    public string HashString(string input)
    {
        var salt = RandomNumberGenerator.GetBytes(128 / 8);
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: input, salt: salt,
            prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));
        return hash;
    }
}
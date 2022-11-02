using System.Security.Cryptography;
using System.Text;
using Flamma.Auth.Common.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Flamma.Auth.Common.Services;

/// <inheritdoc />
public class Hasher : IHasher
{
    /// <inheritdoc />
    public byte[] MakeSalt() => RandomNumberGenerator.GetBytes(128 / 8);
    
    /// <inheritdoc />
    public string HashString(string input, byte[] salt)
    {
        var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(password: input, salt: salt,
            prf: KeyDerivationPrf.HMACSHA256, iterationCount: 100000, numBytesRequested: 256 / 8));
        return hash;
    }
}
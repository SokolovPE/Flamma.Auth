namespace Flamma.Auth.Interfaces;

/// <summary>
///     Hashes data
/// </summary>
public interface IHasher
{
    /// <summary>
    ///     Create HMACSHA256 hash of string
    /// </summary>
    public string HashString(string input);
}
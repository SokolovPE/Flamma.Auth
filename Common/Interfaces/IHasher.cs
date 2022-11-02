namespace Flamma.Auth.Common.Interfaces;

/// <summary>
///     Hashes data
/// </summary>
public interface IHasher
{
    /// <summary>
    ///     Salt generator
    /// </summary>
    /// <returns></returns>
    public byte[] MakeSalt();
    
    /// <summary>
    ///     Create HMACSHA256 hash of string
    /// </summary>
    public string HashString(string input, byte[] salt);
}
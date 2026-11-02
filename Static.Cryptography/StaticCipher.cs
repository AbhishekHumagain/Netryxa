using Static.Cryptography.CipherConfig;
using Static.Cryptography.Ciphers;
using Static.Cryptography.Enums;
using Static.Cryptography.Interfaces;

namespace Static.Cryptography;

/// <summary>
/// A factory class to create instances of cipher implementations based on specified algorithms.
/// </summary>
public static class StaticCipher
{
    /// <summary>
    /// Creates and returns an instance of a cipher implementation based on the specified algorithm.
    /// </summary>
    /// <param name="cipherConfig">An optional configuration object to customize the cipher instance. 
    /// If not provided, default configuration values will be used.</param>
    /// <param name="algorithm">The algorithm to use for the cipher instance. Defaults to <see cref="Algorithms.AesSha256"/>.</param>
    /// <returns>An instance of <see cref="ICipherInstance"/> corresponding to the specified algorithm.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported algorithm is specified.</exception>
    public static ICipherInstance GetCipher(ICipherConfig? cipherConfig = null, Algorithms algorithm = Algorithms.AesSha256) =>
        algorithm switch
        {
            Algorithms.AesSha256 => new AesSha256Cipher(cipherConfig as AesCipherConfig),
            Algorithms.HmacSha256 => new HmacSha256Cipher(cipherConfig as HmacCipherConfig),
            _ => throw new ArgumentOutOfRangeException($"Unsupported algorithm: {algorithm}")
        };
}
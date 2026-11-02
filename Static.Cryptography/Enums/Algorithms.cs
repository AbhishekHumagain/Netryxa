namespace Static.Cryptography.Enums;

/// <summary>
/// Defines the supported encryption algorithms for use with the <see cref="StaticCipher"/> factory.
/// </summary>
public enum Algorithms
{
    /// <summary>
    /// Represents the AES encryption algorithm using a key derived via SHA-256.
    /// This algorithm provides a balance of strong encryption and compatibility.
    /// </summary>
    AesSha256 = 1,
    HmacSha256 = 2
}

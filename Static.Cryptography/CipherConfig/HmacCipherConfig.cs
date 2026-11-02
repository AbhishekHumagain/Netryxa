using Static.Cryptography.Interfaces;

namespace Static.Cryptography.CipherConfig;

/// <summary>
/// Represents the configuration settings required for HMAC-SHA256 verification.
/// </summary>
public class HmacCipherConfig : ICipherConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HmacCipherConfig"/> class.
    /// </summary>
    public HmacCipherConfig() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HmacCipherConfig"/> class with the specified secret key.
    /// </summary>
    /// <param name="secretKey">The secret key used for generating/verifying HMAC signatures.</param>
    public HmacCipherConfig(string secretKey)
    {
        SecretKey = secretKey;
    }

    /// <summary>
    /// Gets the secret key used in HMAC generation or verification.
    /// </summary>
    public string SecretKey { get; init; }
    
    /// <summary>
    /// Gets the client signature used in HMAC generation or verification.
    /// </summary>
    public string ClientSignature { get; init; }
}
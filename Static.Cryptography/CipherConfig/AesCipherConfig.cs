using Static.Cryptography.Interfaces;

namespace Static.Cryptography.CipherConfig;

/// <summary>
/// Represents the configuration settings required for AES encryption and decryption.
/// </summary>
public sealed class AesCipherConfig : ICipherConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AesCipherConfig"/> class with default values.
    /// </summary>
    public AesCipherConfig()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AesCipherConfig"/> class with specified configuration parameters.
    /// </summary>
    /// <param name="salt">The salt value used for key derivation, improving encryption security.</param>
    /// <param name="keySize">The key size (in bits) for the AES algorithm. Typically, 128, 192, or 256.</param>
    /// <param name="passPhrase">The passphrase used to derive the encryption key.</param>
    /// <param name="initVectorBytes">The initialization vector (IV) used for AES encryption in CBC mode.</param>
    public AesCipherConfig(byte[] salt, int keySize, string passPhrase, byte[] initVectorBytes)
    {
        Salt = salt;
        KeySize = keySize;
        PassPhrase = passPhrase;
        InitVectorBytes = initVectorBytes;
    }

    /// <summary>
    /// Gets the passphrase used to derive the encryption key.
    /// </summary>
    public string PassPhrase { get; init; }

    /// <summary>
    /// Gets the salt value used for key derivation. 
    /// Adding a salt ensures that the same passphrase generates a different key for each salt.
    /// </summary>
    public byte[] Salt { get; init; }

    /// <summary>
    /// Gets the key size (in bits) for the AES algorithm.
    /// Supported values are 128, 192, or 256 bits.
    /// </summary>
    public int KeySize { get; init; }

    /// <summary>
    /// Gets the initialization vector (IV) used for AES encryption in CBC mode.
    /// The IV ensures that identical plaintext inputs produce different ciphertexts.
    /// </summary>
    public byte[] InitVectorBytes { get; init; }
}

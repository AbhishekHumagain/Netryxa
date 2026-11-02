namespace Static.Cryptography.Interfaces;

/// <summary>
/// Defines the contract for encryption and decryption operations in a cipher.
/// </summary>
public interface ICipherInstance
{
    /// <summary>
    /// Encrypts the provided plaintext into a secure ciphertext string.
    /// </summary>
    /// <param name="plainText">The plaintext string to encrypt.</param>
    /// <returns>
    /// A task representing the asynchronous encryption operation. 
    /// The result is a Base64-encoded string representing the encrypted ciphertext.
    /// </returns>
    Task<string> Encrypt(string plainText);

    /// <summary>
    /// Decrypts the provided ciphertext back into plaintext.
    /// </summary>
    /// <param name="cipherText">The Base64-encoded ciphertext string to decrypt.</param>
    /// <returns>
    /// A task representing the asynchronous decryption operation. 
    /// The result is the original plaintext string.
    /// </returns>
    /// **Note**: For one-way encryption algorithms (e.g., hashing), this method 
    /// should not be implemented as the data cannot and should not be decrypted.
    /// Instead, consider using a separate interface for hashing or marking this 
    /// method with a `NotImplementedException`.
    Task<string> Decrypt(string cipherText);
}

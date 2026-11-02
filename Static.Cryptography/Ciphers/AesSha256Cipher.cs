using Static.Cryptography.CipherConfig;
using Static.Cryptography.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Static.Cryptography.Ciphers;

/// <summary>
/// Implements AES encryption and decryption using a key derived from SHA-256.
/// The cipher uses CBC mode for encryption and decryption.
/// </summary>
public class AesSha256Cipher(AesCipherConfig? cipherConfig) : BaseCipher(new AesCipherConfig(
    salt: cipherConfig?.Salt ?? CipherDefaults.AesSha256Defaults.Salt,
    keySize: cipherConfig != null && SupportedKeySizes.Contains(cipherConfig.KeySize)
        ? cipherConfig.KeySize
        : CipherDefaults.AesSha256Defaults.KeySize,
    passPhrase: cipherConfig?.PassPhrase ?? CipherDefaults.AesSha256Defaults.PassPhrase,
    initVectorBytes: cipherConfig?.InitVectorBytes ?? CipherDefaults.AesSha256Defaults.InitVectorBytes)), ICipherInstance
{
    /// <summary>
    /// Supported key sizes for the AES encryption algorithm (in bits).
    /// </summary>
    private static readonly int[] SupportedKeySizes = [128, 192, 256];

    /// <summary>
    /// Encrypts the given plain text using AES with an SHA-256 derived key.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <returns>The encrypted text, encoded in Base64 format.</returns>
    public async Task<string> Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        var aesCipherConfig = (CipherConfig as AesCipherConfig)!;
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

        using (var sha256 = SHA256.Create())
        {
            // Derive the encryption key using SHA-256.
            var saltWithPassword = aesCipherConfig.Salt.Concat(Encoding.UTF8.GetBytes(aesCipherConfig.PassPhrase)).ToArray();
            var keyBytes = sha256.ComputeHash(saltWithPassword).Take(aesCipherConfig.KeySize / 8).ToArray();

            using (var aes = Aes.Create())
            {
                aes.Mode = CipherMode.CBC;
                using (var encryptor = aes.CreateEncryptor(keyBytes, aesCipherConfig.InitVectorBytes))
                {
                    await using (var memoryStream = new MemoryStream())
                    {
                        await using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            await cryptoStream.FlushFinalBlockAsync();
                            return Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Decrypts the given encrypted text using AES with a SHA-256 derived key.
    /// </summary>
    /// <param name="cipherText">The Base64-encoded encrypted text to decrypt.</param>
    /// <returns>The decrypted plain text.</returns>
    public async Task<string> Decrypt(string cipherText)
    {
        try
        {
            if (string.IsNullOrEmpty(cipherText))
                return string.Empty;

            var aesCipherConfig = (CipherConfig as AesCipherConfig)!;
            var cipherTextBytes = Convert.FromBase64String(cipherText);

            using (var sha256 = SHA256.Create())
            {
                // Derive the decryption key using SHA-256.
                var saltWithPassword = aesCipherConfig.Salt.Concat(Encoding.UTF8.GetBytes(aesCipherConfig.PassPhrase))
                                                      .ToArray();
                var keyBytes = sha256.ComputeHash(saltWithPassword).Take(aesCipherConfig.KeySize / 8).ToArray();

                using (var aes = Aes.Create())
                {
                    aes.Mode = CipherMode.CBC;
                    using (var decryptor = aes.CreateDecryptor(keyBytes, aesCipherConfig.InitVectorBytes))
                    {
                        await using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            await using (var cryptoStream =
                                         new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var decryptedBytes = new byte[cipherTextBytes.Length];
                                int bytesRead;
                                int totalBytesRead = 0;

                                // Read the decrypted data.
                                while ((bytesRead = await cryptoStream.ReadAsync(
                                           decryptedBytes.AsMemory(totalBytesRead,
                                               decryptedBytes.Length - totalBytesRead))) > 0)
                                {
                                    totalBytesRead += bytesRead;
                                }

                                return Encoding.UTF8.GetString(decryptedBytes, 0, totalBytesRead);
                            }
                        }
                    }
                }
            }
        }
        catch(Exception)
        {
            return string.Empty;
        }
    }
}
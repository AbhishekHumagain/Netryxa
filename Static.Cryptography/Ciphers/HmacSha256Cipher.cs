using Static.Cryptography.CipherConfig;
using Static.Cryptography.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Static.Cryptography.Ciphers;

public class HmacSha256Cipher : ICipherInstance
{
    private readonly string _secretKey;
    private readonly string _clientSignature;

    public HmacSha256Cipher(HmacCipherConfig? config)
    {
        if (config == null || string.IsNullOrWhiteSpace(config.SecretKey))
            throw new ArgumentNullException(nameof(config.SecretKey));

        if (config == null || string.IsNullOrWhiteSpace(config.ClientSignature))
            throw new ArgumentNullException(nameof(config.ClientSignature));

        _clientSignature = config.ClientSignature;
        _secretKey = config.SecretKey;
    }
    
    public async Task<string> Encrypt(string plainText)
    {
        var keyBytes = Encoding.UTF8.GetBytes(_secretKey);
        var messageBytes = Encoding.UTF8.GetBytes(plainText);

        using var hmacSha256 = new HMACSHA256(keyBytes);
        var hashBytes = hmacSha256.ComputeHash(messageBytes);
        var signature = Convert.ToBase64String(hashBytes);

        if (signature != _clientSignature)
            return string.Empty;

        var valueBytes = Convert.FromBase64String(plainText);
        return Encoding.UTF8.GetString(valueBytes);
    }

    public Task<string> Decrypt(string cipherText) => throw new NotImplementedException();
}

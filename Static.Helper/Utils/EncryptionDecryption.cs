using System.Security.Cryptography;
using System.Text;

namespace Static.Helper.Utils;

public static class EncryptionDecryption
{
    public static async Task<string> EncryptString(string key, string plainText)
    {
        var iv = new byte[16];
        byte[] array;
   
        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
   
            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
   
            using (var memoryStream = new MemoryStream())
            {
                await using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    await using (var streamWriter = new StreamWriter(cryptoStream))
                    {
                        await streamWriter.WriteAsync(plainText);
                    }
   
                    array = memoryStream.ToArray();
                }
            }
        }
   
        return Convert.ToBase64String(array);
    }
    
    public static  async Task<string> DecryptString(string key, string cipherText)
    {
        var iv = new byte[16];
        var buffer = Convert.FromBase64String(cipherText);
   
        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
   
            using (var memoryStream = new MemoryStream(buffer))
            {
                await using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (var streamReader = new StreamReader(cryptoStream))
                    {
                        return await streamReader.ReadToEndAsync();
                    }
                }
            }
        }
    }

}
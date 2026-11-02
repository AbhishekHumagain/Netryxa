using Static.Cryptography.Interfaces;

namespace Static.Cryptography.Ciphers;

public abstract class BaseCipher(ICipherConfig cipherConfig)
{
    protected ICipherConfig CipherConfig { get; } = cipherConfig;
}
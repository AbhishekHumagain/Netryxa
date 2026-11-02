using System.Text;

namespace Static.Cryptography;

public static class CipherDefaults
{
    public static class AesSha256Defaults
    {
        public static byte[] Salt { get; set; } = Encoding.ASCII.GetBytes("hgt!16kl");
        public static int KeySize { get; set; } = 256;
        public static string PassPhrase { get; set; } = "svf64DI1b0pg5wz";
        public static byte[] InitVectorBytes { get; set; } = Encoding.ASCII.GetBytes("jkE49230Tf093b42");
    }
}

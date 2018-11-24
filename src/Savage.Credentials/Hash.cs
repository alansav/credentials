using System.Security.Cryptography;

namespace Savage.Credentials
{
    public static class Hash
    {
        public static byte[] HashBytes(byte[] data)
        {
            var sha = SHA512.Create();
            return sha.ComputeHash(data);
        }
    }
}

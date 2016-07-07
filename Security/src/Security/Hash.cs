using System.Security.Cryptography;

namespace Savage.Security
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

using System.Linq;
using System.Security.Cryptography;

namespace Savage.Credentials
{
    public class SaltAndHashedPassword
    {
        public readonly byte[] Salt;
        public readonly byte[] HashedPassword;

        private SaltAndHashedPassword(byte[] salt, byte[] hashedPassword)
        {
            Salt = salt;
            HashedPassword = hashedPassword;
        }

        public static SaltAndHashedPassword New(string password)
        {
            var salt = RandomBytesGenerator.Generate(16);
            return new SaltAndHashedPassword(salt, HashPassword(salt, password));
        }

        public static SaltAndHashedPassword Load(byte[] salt, byte[] hashedPassword)
        {
            return new SaltAndHashedPassword(salt, hashedPassword);
        }
        
        private static byte[] HashPassword(byte[] salt, string password)
        {
            var passwordDeriveBytes = new Rfc2898DeriveBytes(password, salt, 1024);
            var key = passwordDeriveBytes.GetBytes(256);
            return Hash.HashBytes(key);
        }

        public bool ComparePassword(string password)
        {
            return HashedPassword.SequenceEqual(HashPassword(Salt, password));
        }
    }
}

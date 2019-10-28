using System;
using System.Linq;
using System.Security.Cryptography;

namespace Savage.Credentials
{
    public class SaltAndHashedPassword
    {
        public readonly byte[] Salt;
        public readonly byte[] HashedPassword;
        public readonly int Iterations;

        private SaltAndHashedPassword(byte[] salt, byte[] hashedPassword, int iterations)
        {
            Salt = salt;
            HashedPassword = hashedPassword;
            Iterations = iterations;
        }

        public static SaltAndHashedPassword New(string password, int saltLength = 16, int iterations = 1024)
        {
            if (password == string.Empty)
                throw new ArgumentException(nameof(password));

            var salt = RandomBytesGenerator.Generate(saltLength);
            return new SaltAndHashedPassword(salt, HashPassword(salt, password, iterations), iterations);
        }

        public static SaltAndHashedPassword Load(byte[] salt, byte[] hashedPassword, int iterations = 1024)
        {
            return new SaltAndHashedPassword(salt, hashedPassword, iterations);
        }

        public static SaltAndHashedPassword Load(string passwordHashAsString)
        {
            var elements = passwordHashAsString.Split('$');
            if (elements.Length != 5)
            {
                throw new ArgumentException($"Unable to parse: {nameof(passwordHashAsString)}");
            }
            var salt = Convert.FromBase64String(elements[2]);
            var iterations = int.Parse(elements[3]);
            var hashedPassword = Convert.FromBase64String(elements[4]);

            return Load(salt, hashedPassword, iterations);
        }
        
        private static byte[] HashPassword(byte[] salt, string password, int iterations)
        {
            using (var passwordDeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                var key = passwordDeriveBytes.GetBytes(256);
                return Hash.HashBytes(key);
            }
        }

        public bool ComparePassword(string password)
        {
            var calculatedHash = HashPassword(Salt, password, Iterations);
            return HashedPassword.SequenceEqual(calculatedHash);
        }

        public override string ToString()
        {
            return $"$rfc2898${Convert.ToBase64String(Salt)}${Iterations}${Convert.ToBase64String(HashedPassword)}";
        }
    }
}

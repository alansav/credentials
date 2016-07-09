using System;
using System.Linq;

namespace Savage.Credentials
{
    public class Token
    {
        public readonly byte[] ClearTextToken;

        private Token(byte[] clearTextToken)
        {
            ClearTextToken = clearTextToken;
        }

        public static Token Create(int length)
        {
            return new Token(RandomBytesGenerator.Generate(length));
        }

        public static Token Load(byte[] clearTextToken)
        {
            if (clearTextToken == null)
                throw new ArgumentNullException(nameof(clearTextToken));

            return new Token(clearTextToken);
        }
        
        public byte[] HashToken()
        {
            return Hash.HashBytes(ClearTextToken);
        }

        public bool CompareToken(byte[] expectedHashedToken)
        {
            var actualHashedToken = HashToken();
            return expectedHashedToken.SequenceEqual(actualHashedToken);
        }
    }
}

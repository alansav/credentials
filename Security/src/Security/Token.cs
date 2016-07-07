namespace Savage.Security
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
            return new Token(clearTextToken);
        }
        
        public byte[] GetHashedToken()
        {
            return Hash.HashBytes(ClearTextToken);
        }
    }
}

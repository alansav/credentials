namespace Savage.Credentials
{
    public class TokenService : ITokenService
    {
        public Token CreateToken(int length)
        {
            return Token.Create(length);
        }

        public Token LoadToken(byte[] clearTextToken)
        {
            return Token.Load(clearTextToken);
        }
    }
}

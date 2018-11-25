namespace Savage.Credentials
{
    public interface ITokenService
    {
        Token CreateToken(int length);
        Token LoadToken(byte[] bytes);
    }

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

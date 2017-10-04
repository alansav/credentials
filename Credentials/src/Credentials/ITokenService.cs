namespace Savage.Credentials
{
    public interface ITokenService
    {
        Token CreateToken(int length);
        Token LoadToken(byte[] bytes);
    }
}

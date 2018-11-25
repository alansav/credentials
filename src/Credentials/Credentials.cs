namespace Savage.Credentials
{
    public interface ICredentials
    {
        string Username { get; }
        string Password { get; }
        SaltAndHashedPassword CreateSaltAndHashedPassword();
    }

    public class Credentials : ICredentials
    {
        public readonly string Username;
        public readonly string Password;

        public Credentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public SaltAndHashedPassword CreateSaltAndHashedPassword()
        {
            return SaltAndHashedPassword.New(Password);
        }
    }
}

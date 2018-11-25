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
        public string Username { get; private set; }
        public string Password { get; private set; }

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

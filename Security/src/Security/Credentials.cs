namespace Savage.Security
{
    public class Credentials
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

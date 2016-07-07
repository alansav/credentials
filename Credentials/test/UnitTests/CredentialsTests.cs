using Xunit;

namespace Savage.Credentials
{
    public class CredentialsTests
    {
        static Credentials _subject;
        static string _username = "fred@example.org";
        static string _password = "password123";

        public CredentialsTests()
        {
            _subject = new Credentials(_username, _password);
        }

        [Fact]
        public void TestUsernameInitialized()
        {
            Assert.Equal(_username, _subject.Username);
        }

        [Fact]
        public void TestPasswordInitialized()
        {
            Assert.Equal(_password, _subject.Password);
        }

        [Fact]
        public void TestGetSaltAndHashedPassword()
        {
            var saltAndHashedPassword = _subject.CreateSaltAndHashedPassword();
            Assert.True(saltAndHashedPassword.ComparePassword(_password));
        }
    }
}

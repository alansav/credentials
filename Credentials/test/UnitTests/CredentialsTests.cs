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
        public void Username_Should_be_initialized()
        {
            Assert.Equal(_username, _subject.Username);
        }

        [Fact]
        public void Password_Should_be_initialized()
        {
            Assert.Equal(_password, _subject.Password);
        }

        [Fact]
        public void CreateSaltAndHashedPassword_Should_return_a_salt_and_hashed_password_for_the_password()
        {
            var saltAndHashedPassword = _subject.CreateSaltAndHashedPassword();
            Assert.True(saltAndHashedPassword.ComparePassword(_password));
        }
    }
}

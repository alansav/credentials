using Xunit;

namespace Savage.Credentials
{
    public class SaltAndHashedPasswordTests
    {
        [Fact]
        public void TestConstructor()
        {
            var subject = SaltAndHashedPassword.New("password");

            var compare = SaltAndHashedPassword.New("password");

            Assert.NotEqual(subject.Salt, compare.Salt);
            Assert.NotEqual(subject.HashedPassword, compare.HashedPassword);
        }

        [Fact]
        public void TestPasswordIsCaseSensitive()
        {
            var subject = SaltAndHashedPassword.New("password");

            Assert.False(subject.ComparePassword("PASSWORD"));
            Assert.True(subject.ComparePassword("password"));
        }

        [Fact]
        public void TestLoadSaltAndHashedPassword()
        {
            var initial = SaltAndHashedPassword.New("password");

            var subject = SaltAndHashedPassword.Load(initial.Salt, initial.HashedPassword);

            Assert.Equal(initial.Salt, subject.Salt);
            Assert.Equal(initial.HashedPassword, subject.HashedPassword);

            Assert.True(subject.ComparePassword("password"));
        }
    }
}

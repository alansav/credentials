using Xunit;

namespace Savage.Credentials
{
    public class SaltAndHashedPasswordTests
    {
        [Fact]
        public void New_Should_create_SaltAndHashedPassword_with_different_values_for_Salt_when_same_password_is_used()
        {
            var subject = SaltAndHashedPassword.New("password");

            var compare = SaltAndHashedPassword.New("password");

            Assert.NotEqual(subject.Salt, compare.Salt);
        }

        [Fact]
        public void New_Should_create_SaltAndHashedPassword_with_different_values_for_HashedPassword_when_same_password_is_used()
        {
            var subject = SaltAndHashedPassword.New("password");

            var compare = SaltAndHashedPassword.New("password");
            
            Assert.NotEqual(subject.HashedPassword, compare.HashedPassword);
        }

        [Fact]
        public void ComparePassword_Should_return_true_when_password_matches_password_when_created()
        {
            var subject = SaltAndHashedPassword.New("password");
            
            Assert.True(subject.ComparePassword("password"));
        }

        [Fact]
        public void ComparePassword_Should_return_false_when_password_casing_differs()
        {
            var subject = SaltAndHashedPassword.New("password");

            Assert.False(subject.ComparePassword("PASSWORD"));
        }

        [Fact]
        public void LoadSaltAndHashedPassword_Should_Initialize_Salt()
        {
            var initial = SaltAndHashedPassword.New("password");

            var subject = SaltAndHashedPassword.Load(initial.Salt, initial.HashedPassword);

            Assert.Equal(initial.Salt, subject.Salt);
        }

        [Fact]
        public void LoadSaltAndHashedPassword_Should_Initialize_HashedPassword()
        {
            var initial = SaltAndHashedPassword.New("password");

            var subject = SaltAndHashedPassword.Load(initial.Salt, initial.HashedPassword);

            Assert.Equal(initial.HashedPassword, subject.HashedPassword);
        }

        [Fact]
        public void ComparePassword_Should_return_true_when_password_matches_when_loaded()
        {
            var initial = SaltAndHashedPassword.New("password");

            var subject = SaltAndHashedPassword.Load(initial.Salt, initial.HashedPassword);

            Assert.True(subject.ComparePassword("password"));
        }

        [Fact]
        public void ComparePassword_Should_return_false_when_password_does_not_match_when_loaded()
        {
            var initial = SaltAndHashedPassword.New("password");

            var subject = SaltAndHashedPassword.Load(initial.Salt, initial.HashedPassword);

            Assert.False(subject.ComparePassword("incorrect"));
        }
    }
}

using System;
using Xunit;

namespace Savage.Credentials
{
    public class SaltAndHashedPasswordTests
    {
        [Fact]
        public void New_Should_throw_exception_when_password_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => SaltAndHashedPassword.New(null));
        }

        [Fact]
        public void New_Should_throw_exception_when_password_is_empty_string()
        {
            Assert.Throws<ArgumentException>(() => SaltAndHashedPassword.New(String.Empty));
        }

        [Fact]
        public void New_Should_throw_exception_when_salt_length_is_zero()
        {
            Assert.Throws<ArgumentException>(() => SaltAndHashedPassword.New("password", 0));
        }

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
        public void New_Should_create_SaltAndHashedPassword_with_default_salt_length_of_16()
        {
            var subject = SaltAndHashedPassword.New("password");

            Assert.True(subject.Salt.Length == 16);
        }

        [Fact]
        public void New_Should_create_SaltAndHashedPassword_with_specified_salt_length()
        {
            var subject = SaltAndHashedPassword.New("password", 24);

            Assert.True(subject.Salt.Length == 24);
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

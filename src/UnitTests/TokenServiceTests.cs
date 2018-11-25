using System;
using Xunit;

namespace Savage.Credentials
{
    public class TokenServiceTests
    {
        [Fact]
        public void CreateToken_should_return_Token_of_expected_length()
        {
            var service = new TokenService();
            var token = service.CreateToken(16);
            Assert.Equal(16, token.ClearTextToken.Length);
        }

        [Fact]
        public void LoadToken_should_return_expected_Token()
        {
            var service = new TokenService();
            var token = service.LoadToken(Convert.FromBase64String("IF+A58dd413xT3wQIMLo3A=="));
            Assert.Equal("we+IxS22HNsRYxL94DR23XQvIYgqb+QF3lOLoS0ToYfW7UIC8EB/ipN4k5iMXaMhHwtva52OQwLiR08fxNxMkw==", Convert.ToBase64String(token.HashToken()));
        }
        
        [Fact]
        public void CreateToken_Should_throw_exception_when_length_is_zero()
        {
            var service = new TokenService();
            Assert.Throws<ArgumentException>(() => service.CreateToken(0));
        }

        [Fact]
        public void LoadToken_Should_throw_exception_when_clearTextToken_is_null()
        {
            var service = new TokenService();
            Assert.Throws<ArgumentNullException>(() => service.LoadToken(null));
        }

        [Fact]
        public void LoadToken_Should_throw_exception_when_clearTextToken_is_empty()
        {
            var service = new TokenService();
            var emptyBytes = new byte[0];
            Assert.Throws<ArgumentException>(() => service.LoadToken(emptyBytes));
        }
    }
}

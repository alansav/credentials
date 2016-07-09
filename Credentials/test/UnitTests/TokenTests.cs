using System;
using Xunit;

namespace Savage.Credentials
{
    public class TokenTests
    {
        [Fact]
        public void TestCreate()
        {
            var subject = Token.Create(16);
            Assert.Equal(16, subject.ClearTextToken.Length);
        }

        [Fact]
        public void TestLoad()
        {
            var subject = Token.Load(Convert.FromBase64String("IF+A58dd413xT3wQIMLo3A=="));
            Assert.Equal("we+IxS22HNsRYxL94DR23XQvIYgqb+QF3lOLoS0ToYfW7UIC8EB/ipN4k5iMXaMhHwtva52OQwLiR08fxNxMkw==", Convert.ToBase64String(subject.HashToken()));
        }

        [Fact]
        public void CompareToken_Should_return_true_when_token_matches()
        {
            var subject = Token.Load(Convert.FromBase64String("IF+A58dd413xT3wQIMLo3A=="));
            var expectedHashedToken = Convert.FromBase64String("we+IxS22HNsRYxL94DR23XQvIYgqb+QF3lOLoS0ToYfW7UIC8EB/ipN4k5iMXaMhHwtva52OQwLiR08fxNxMkw==");

            Assert.True(subject.CompareToken(expectedHashedToken));
        }

        [Fact]
        public void CompareToken_Should_return_false_when_token_matches()
        {
            var subject = Token.Load(Convert.FromBase64String("IF+A58dd413xT3wQIMLo3B=="));
            var expectedHashedToken = Convert.FromBase64String("we+IxS22HNsRYxL94DR23XQvIYgqb+QF3lOLoS0ToYfW7UIC8EB/ipN4k5iMXaMhHwtva52OQwLiR08fxNxMkw==");

            Assert.True(subject.CompareToken(expectedHashedToken));
        }
    }
}

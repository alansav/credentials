using System;
using Xunit;

namespace Savage.Credentials
{
    public class HashTests
    {
        [Fact]
        public void HashBytes_Should_use_sha512()
        {
            byte[] data = Convert.FromBase64String("bw3C6yRwUkVmm6dngM6udw==");
           
            var a = Hash.HashBytes(data);
            Assert.Equal("tHBEZYOVpORG3IbmMcrbxd30A4QkbYZ+E3DzavTMOPc+beaJzeKE1/INjBDkvGAlQnhLGzbrk662h1HHx6dPRw==", Convert.ToBase64String(a));
        }
    }
}

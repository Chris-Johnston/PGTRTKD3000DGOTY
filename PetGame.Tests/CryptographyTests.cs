using PetGame.Models;
using PetGame.Util;
using System;
using Xunit;

namespace PetGame.Tests
{
    /// <summary>
    ///     Tests the user login behavior.
    /// </summary>
    public class CryptographyTests
    {
        [Fact]
        public void TestLogin()
        {
            var user = new User()
            {
                UserId = 0,
                Username = "Test Dude"
            };
            // set the user's password and hmac key
            Cryptography.SetUserPassword(user, "Password123");

            var pw1 = new byte[user.PasswordHash.Length];
            Array.Copy(user.PasswordHash, pw1, user.PasswordHash.Length);

            Assert.True(Cryptography.VerifyUserPassword(user, "Password123"));
            Assert.False(Cryptography.VerifyUserPassword(user, "Password123!"));

            // if the HMAC key is set again, all passwords will be invalidated
            Cryptography.SetUserHMACKey(user);

            Assert.False(Cryptography.VerifyUserPassword(user, "Password123"));

            Cryptography.SetUserPassword(user, "Password123");

            Assert.True(Cryptography.VerifyUserPassword(user, "Password123"));

            // assert that the password hash is different because the hmac keys have changed too
            Assert.False(Cryptography.CryptographicCompare(pw1, user.PasswordHash));
        }

        [Fact]
        public void TestSetUserHMACKey()
        {
            // hash and hmac key are going to be null by default
            var user = new User() { Username = "AAAA" };
            Assert.Null(user.PasswordHash);
            Assert.Null(user.HMACKey);

            Cryptography.SetUserHMACKey(user);

            Assert.NotNull(user.HMACKey);
            Assert.Null(user.PasswordHash);

            var k = user.HMACKey;

            // set a new hmac
            Cryptography.SetUserHMACKey(user);
            // keys should not be the same
            Assert.False(Cryptography.CryptographicCompare(k, user.HMACKey));
        }

        [Theory]
        [InlineData(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }, true)]
        [InlineData(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 4 }, false)]
        [InlineData(new byte[] { 1, 2, 3 }, new byte[] { 1 }, false)]
        [InlineData(new byte[] { 1 }, new byte[] { 1, 2, 3 }, false)]
        [InlineData(new byte[] { 1 }, null, false)]
        [InlineData(null, null, false)]
        [InlineData(null, new byte[] { 10 }, false)]
        public void TestCryptographicEquals(byte[] a, byte[] b, bool eq)
        {
            Assert.Equal(eq, Cryptography.CryptographicCompare(a, b));
        }
    }
}

using PetGame.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PetGame.Tests
{
    /// <summary>
    ///     Tests the behavior of the <see cref="User"/> class.
    /// </summary>
    public class UserTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        [InlineData("\t")]
        public void TestNullOrWhitespaceUsernames(string name)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var user = new User()
                {
                    Username = name
                };
            });
        }

        [Theory]
        [InlineData("a")]
        [InlineData("‽")]
        [InlineData("s p a c e s")]
        [InlineData(" invalid")]
        [InlineData("invalid   ")]
        [InlineData("<script>alert(1);</script>")]
        [InlineData("Bobby 'DROP TABLE [User];--")]
        // 50 char
        [InlineData("ljkasdfljkafsdjklfjkdjklfadsjkladfsjkladfsjklafdsj")]
        // 51 char
        [InlineData("ljkasdfljkafsdjklfjkdjklfadsjkladfsjkladfsjklafdsja")]
        public void TestInvalidUsernames(string name)
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var user = new User()
                {
                    Username = name
                };
            });
        }

        [Theory]
        [InlineData("chris")]
        [InlineData("hackerman")]
        [InlineData("hackerman!!!")]
        [InlineData("aa")]
        [InlineData("ORANG")]
        [InlineData("aAaAaAaA")]
        [InlineData("!!!!!")]
        [InlineData("!?")]
        [InlineData("1337")]
        public void TestValidUsernames(string name)
        {
            _ = new User()
            {
                Username = name
            };
        }
    }
}

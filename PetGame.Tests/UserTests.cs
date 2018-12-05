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
        // 50 char
        [InlineData("ljkasdfljkafsdjklfjkdjklfadsjkladfsjkladfsjklafdsj")]
        public void TestValidUsernames(string name)
        {
            _ = new User()
            {
                Username = name
            };
        }

        [Theory]
        // valid number
        [InlineData("+11231231234", true)]
        // null ok
        [InlineData(null, true)]
        // empty / whitespace not ok
        [InlineData("", false)]
        [InlineData("  ", false)]
        [InlineData("1231231234", false)]
        // missing one char
        [InlineData("+1231231234", false)]
        [InlineData("+10000000000", true)]
        // no special symbols
        // TODO: Consider having the API normalize phone numbers before hitting the DB.
        [InlineData("+1000000-0000", false)]
        [InlineData("+1(000)000-0000", false)]
        [InlineData("+1 (000) 000-0000", false)]
        [InlineData("+19999999999", true)]
        // one char too long
        [InlineData("+199999999991", false)]
        // no trailing/leading spaces
        [InlineData("+19999999999 ", false)]
        [InlineData(" +19999999999 ", false)]
        [InlineData(" +19999999999", false)]
        public void TestValidPhoneNumbers(string phoneNumber, bool valid)
        {
            if (valid)
            {
                // create the user, should not throw exception
                _ = new User()
                {
                    PhoneNumber = phoneNumber
                };
            }
            else
            {
                var e = Assert.ThrowsAny<Exception>(() =>
                {
                    // create the user, should throw exception
                    _ = new User()
                    {
                        PhoneNumber = phoneNumber
                    };
                });
                Assert.Contains(e.GetType(), new List<Type>()
                {
                    typeof(ArgumentException)
                    // can check for different types, but currently only this one.
                });
            }
        }
    }
}

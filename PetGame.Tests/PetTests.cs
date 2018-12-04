using PetGame.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PetGame.Tests
{
    /// <summary>
    ///     Tests for the <see cref="Pet"/> class
    /// </summary>
    public class PetTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void TestInvalidEndurance(int endurance)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var p = new Pet()
                {
                    Endurance = endurance
                };
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(99)]
        [InlineData(100)]
        public void TestValidEndurance(int endurance)
        {
            // should not throw exception
            _ = new Pet()
            {
                Endurance = endurance
            };
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(101)]
        public void TestInvalidStrength(int strength)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var p = new Pet()
                {
                    Strength = strength
                };
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(99)]
        [InlineData(100)]
        public void TestValidStrength(int strength)
        {
            // should not throw exception
            _ = new Pet()
            {
                Strength = strength
            };
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        // 51 char
        [InlineData("ljkasdfljkafsdjklfjkdjklfadsjkladfsjkladfsjklafdsja")]
        public void TestInvalidPetNames(string name)
        {
            var ex = Assert.ThrowsAny<Exception>(() =>
            {
                _ = new Pet()
                {
                    Name = name
                };
            });

            // check that the exception is one of these types
            Assert.Contains(ex.GetType(), new List<Type>()
            {
                typeof(ArgumentNullException),
                typeof(ArgumentException)
            });
        }

        [Theory]
        [InlineData("fluffy")]
        [InlineData("peeeet")]
        [InlineData("idk")]
        [InlineData("Fluffy Jr")]
        [InlineData("s p a c e s")]
        [InlineData("saam's dragon")]
        // 50 char
        [InlineData("ljkasdfljkafsdjklfjkdjklfadsjkladfsjkladfsjklafdsj")]
        public void TestValidPetNames(string name)
        {
            _ = new Pet()
            {
                Name = name
            };
        }
    }
}

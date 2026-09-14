using System;
using System.Collections.Generic;
using System.Text;

namespace IceCreamMachine.Test
{
    public class IceCreamMachineTests
    {
        [Fact]
        public void GetScoopsShouldReturnFive_ForXLSize()
        {
            // Arrange
            var machine = new IceCreamMachine();
            

            // Act
            var result = machine.GetScoops(size:"XL");


            // Assert
            Assert.Equal(5, result);

        }

        [Fact]
        public void GetScoopsShouldReturnFour_ForLSize()
        {
            // Arrange
            var machine = new IceCreamMachine();


            // Act
            var result = machine.GetScoops(size: "L");


            // Assert
            Assert.Equal(4, result);

        }


        [Theory]
        [InlineData("S", 1)]
        [InlineData("M", 2)]
        [InlineData("L", 4)]
        [InlineData("XL", 5)]
        [InlineData("XXL", 0)]
        [InlineData("ABL", 0)]
        [InlineData("CTO", 0)]
        [InlineData("SFOERWEFK#¤", 0)]
        public void GetScoopsShouldReturnCorrectValue(string size, int expected)
        {
            // Arrange
            var sut = new IceCreamMachine();

            // Act
            var result = sut.GetScoops(size);

            // Assert
            Assert.Equal(expected, result);

        }


        [Fact]
        public void GetPrice_ShouldReturnFifthy_ForXLSize()
        {
            // Arrange
            var sut = new IceCreamMachine();
            // Act
            var price = sut.GetPrice(size: "XL");
            // Assert
            Assert.Equal(50, price);
        }
        [Fact]
        public void GetPrice_ShouldReturnForthy_ForLSize()
        {
            // Arrange
            var sut = new IceCreamMachine();
            // Act
            var price = sut.GetPrice(size: "L");
            // Assert
            Assert.Equal(40, price);
        }

        [Theory]
        [InlineData("S", 10)]
        [InlineData("M", 20)]
        [InlineData("L", 40)]
        [InlineData("XL", 50)]
        [InlineData("XXL", 0)]
        [InlineData("ABL", 0)]
        [InlineData("CTO", 0)]
        public void GetPrice_ShouldReturnCorrectValue(string size, int expected)
        {
            // Arrange
            var sut = new IceCreamMachine();

            // Act
            var result = sut.GetPrice(size);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsValidSize_ShouldReturnTrue_ForM()
        {
            // Arrange
            var sut = new IceCreamMachine();
            // Act
            var result = sut.IsValidSize("M");
            // Assert
            Assert.True(result);
        }
        [Fact]
        public void IsValidSize_ShouldReturnTrue_ForXL()
        {
            // Arrange
            var sut = new IceCreamMachine();
            // Act
            var result = sut.IsValidSize("XL");
            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("S", true)]
        [InlineData("M", true)]
        [InlineData("L", true)]
        [InlineData("XL", true)]
        [InlineData("XXL", false)]
        [InlineData("", false)]
        [InlineData("CTO", false)]
        public void IsValidSize_ShouldReturnExpectedResult(string size, bool expected)
        {
            // Arrange
            var sut = new IceCreamMachine();

            // Act
            var result = sut.IsValidSize(size);

            // Assert
            Assert.Equal(expected, result);
        }

    }
}

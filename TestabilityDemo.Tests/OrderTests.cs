namespace TestabilityDemo.Tests;

using System.Reflection;
using TestabilityDemo;

public class OrderTests
{
    [Fact]
    public void CalculatesPriceWithoutDiscount()
    {
        // Arrange
        var sut = new Order(DateTime.Parse("2026-09-15T00:00:00Z"));
        sut.Item = "test item";
        sut.Price = 4;
        sut.Quantity = 3;
        
        // Act 
        var actual = sut.GetTotal();

        // Assert
        Assert.Equal(actual, 12);
    }

    [Fact]
    public void CalculatesPriceWithDiscountOnFridays()
    {
                // Arrange
        var sut = new Order(DateTime.Parse("2026-09-18T00:00:00Z"));
        sut.Item = "test item";
        sut.Price = 4;
        sut.Quantity = 3;
        
        // Act 
        var actual = sut.GetTotal();

        // Assert
        Assert.Equal(actual, 9);
    }
}

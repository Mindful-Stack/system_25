namespace IceCreamMachine.Tests;

public class IceCreamMachineTests
{
    [Fact]
    public void GetScoops_ShouldReturnFive_ForXLSize()
    {
        // Arrange
        var machine = new IceCreamMachine();

        // Act
        var result = machine.GetScoops("XL");

        // Assert
        Assert.Equal(5, result);
    }
    
    [Fact]
    public void GetScoops_ShouldReturnFour_ForLSize()
    {
        // Arrange
        var machine = new IceCreamMachine();

        // Act
        var result = machine.GetScoops("L");

        // Assert
        Assert.Equal(4, result);
    }

    [Theory]
    [InlineData("S", 1)]
    [InlineData("M", 3)]
    [InlineData("L", 4)]
    [InlineData("XL", 5)]
    [InlineData("XXL", 0)]
    [InlineData("ABL", 0)]
    [InlineData("CTO", 0)]
    [InlineData("@5%6RTS", 0)]
    public void GetScoops_ShouldReturnCorrectNumberOfScoops(string size, int expected)
    {
        var sut = new IceCreamMachine();

        var result = sut.GetScoops(size);
        
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetPrice_ShouldReturnFifty_ForXL()
    {
        var sut = new IceCreamMachine();

        var price = sut.GetPrice("XL");
        
        Assert.Equal(50, price);
    }
    
    [Fact]
    public void GetPrice_ShouldReturnFourty_ForL()
    {
        var sut = new IceCreamMachine();

        var price = sut.GetPrice("L");
        
        Assert.Equal(40, price);
    }

    [Theory]
    [InlineData("S", 10)]
    [InlineData("M", 30)]
    [InlineData("L", 40)]
    [InlineData("XL", 50)]
    [InlineData("XXL", 0)]
    public void GetPrice_ShouldReturnCorrectPrice(string size, int expected)
    {
        var sut = new IceCreamMachine();

        var price = sut.GetPrice(size);
        Assert.Equal(expected, price);
    }

    [Fact]
    public void IsValidSize_ShouldReturnTrue_ForM()
    {
        var sut = new IceCreamMachine();

        var result = sut.IsValidSize("M");
        
        Assert.True(result);
    }
    
    [Fact]
    public void IsValidSize_ShouldReturnFalse_ForXXL()
    {
        var sut = new IceCreamMachine();

        var result = sut.IsValidSize("XXL");
        
        Assert.False(result);
    }

    [Theory]
    [InlineData("S", true)]
    [InlineData("M", true)]
    [InlineData("L", true)]
    [InlineData("XL", true)]
    [InlineData("XXL", false)]
    [InlineData("", false)]
    public void IsValidSize_ShouldReturnExpectedResult(string size, bool expected)
    {
        var sut = new IceCreamMachine();

        var result = sut.IsValidSize(size);
        
        Assert.Equal(expected, result);
    }
}
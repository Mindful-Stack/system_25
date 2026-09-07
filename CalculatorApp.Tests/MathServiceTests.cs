namespace CalculatorApp.Tests;

public class MathServiceTests
{
    private readonly MathService _sut = new MathService();

    // Assert.Equal(expected, actual);
    [Fact]
    public void Add_TwoPlusThree_ReturnsFive()
    {
        // Arrange
        double expected = 5;

        // Act
        double actual = _sut.Add(2, 3);

        // Assert
        Assert.Equal(expected, actual);
    }

    // Assert.Equal(expected, actual, precision);
    [Fact]
    public void GetPi_RoundedToTwoDecimals_IsCloseEnough()
    {
        // Arrange
        double expected = 3.14;

        // Act
        double actual = _sut.GetPi();

        // Assert - only the first two decimals have to match
        Assert.Equal(expected, actual, 2);
    }

    // Assert.InRange(actual, min, max);
    [Fact]
    public void Add_TwoPlusThree_IsInRange()
    {
        // Act
        double actual = _sut.Add(2, 3);

        // Assert
        Assert.InRange(actual, 4, 6);
    }
}

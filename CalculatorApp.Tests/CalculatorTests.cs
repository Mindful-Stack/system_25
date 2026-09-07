namespace CalculatorApp.Tests;

public class CalculatorTests
{
    private readonly Calculator _sut = new Calculator();
    
    [Fact]
    public void Add_TwoPlusTwo_ReturnsFour()
    {
        // Arrange - prepare test data and object
        int a = 2;
        int b = 2;
        int expected = 4;

        // Act - runt the operation
        int actual = _sut.Add(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Subtract_FiveMinusThree_ReturnsTwo()
    {
        // Arrange
        int a = 5;
        int b = 3;
        int expected = 2;

        // Act
        int actual = _sut.Subtract(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Multiply_ThreeTimesFour_ReturnsTwelve()
    {
        // Arrange
        int a = 3;
        int b = 4;
        int expected = 12;

        // Act
        int actual = _sut.Multiply(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Multiply_ByZero_ReturnsZero()
    {
        // Arrange
        int a = 0;
        int b = 5;
        int expected = 0;
        
        // Act
        int actual = _sut.Multiply(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Divide_SixDividedByTwo_ReturnsThree()
    {
        // Arrange
        int a = 6;
        int b = 2;
        double expected = 3.0;

        // Act
        double actual = _sut.Divide(6, 2);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        // Arrange
        int a = 5;
        int b = 0;

        // Act & Assert - we expect an exception
        Assert.Throws<DivideByZeroException>(() => _sut.Divide(a, b));
    }
}
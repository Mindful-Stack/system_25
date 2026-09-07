namespace CalculatorApp.Tests;

public class StringServiceTests
{
    private readonly StringService _sut = new StringService();

    // Assert.Equal(expected, actual);
    [Fact]
    public void Greet_ReturnsExactText()
    {
        // Arrange
        string expected = "Hello World";

        // Act
        string actual = _sut.Greet();

        // Assert
        Assert.Equal(expected, actual);
    }

    // Assert.Equal(expected, actual, ignoreCase: true);
    [Fact]
    public void Echo_UpperCaseInput_MatchesLowerCaseIgnoringCase()
    {
        // Arrange
        string expected = "test";

        // Act
        string actual = _sut.Echo("TEST");

        // Assert
        Assert.Equal(expected, actual, ignoreCase: true);
    }

    // Assert.Contains(expected, actual);
    [Fact]
    public void Greet_ContainsHello()
    {
        // Act
        string actual = _sut.Greet();

        // Assert
        Assert.Contains("Hello", actual);
    }

    // Assert.StartsWith(expected, actual);
    [Fact]
    public void Greet_StartsWithHello()
    {
        // Act
        string actual = _sut.Greet();

        // Assert
        Assert.StartsWith("Hello", actual);
    }

    // Assert.EndsWith(expected, actual);
    [Fact]
    public void Greet_EndsWithWorld()
    {
        // Act
        string actual = _sut.Greet();

        // Assert
        Assert.EndsWith("World", actual);
    }

    // Assert.Matches(pattern, actual);
    [Fact]
    public void Echo_LettersAndDigits_MatchesWordPattern()
    {
        // Act
        string actual = _sut.Echo("Test123");

        // Assert
        Assert.Matches(@"^\w+$", actual);
    }
}

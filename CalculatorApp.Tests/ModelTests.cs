namespace CalculatorApp.Tests;

[Collection(nameof(AdvancedCalculatorCollection))]
public class ModelTests
{
    private readonly AdvancedCalculatorFixture _fixture;

    public ModelTests(AdvancedCalculatorFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Model_ContainsTI()
    {
        // Arrange
        var expectedSubstring = "TI";

        // Act
        var actual = _fixture.Calculator.Model;

        // Assert
        Assert.Contains(expectedSubstring, actual);
    }

    [Fact]
    public void IsScientific_IsTrue() => Assert.True(_fixture.Calculator.IsScientific);
}
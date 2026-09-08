namespace CalculatorApp.Tests;

[Collection(nameof(AdvancedCalculatorCollection))]
public class OwnerTests(AdvancedCalculatorFixture fixture)
{
    private readonly AdvancedCalculatorFixture _fixture = fixture;

    [Fact]
    public void Owner_IsDaniel()
    {
        // Arrange
        var expected = "Daniel";

        // Act
        var actual = _fixture.Calculator.Owner;

        // Assert
        Assert.Equal(expected, actual);
    }
}
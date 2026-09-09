namespace CalculatorApp.Tests;

public class AdvancedCalculatorTests : IClassFixture<AdvancedCalculatorFixture>
{
    private readonly AdvancedCalculatorFixture _calculatorFixture;
    private readonly AdvancedCalculator _sut;

    public AdvancedCalculatorTests(AdvancedCalculatorFixture calculatorFixture)
    {
        _calculatorFixture = calculatorFixture;
        _sut = _calculatorFixture.Calculator;
    }

    [Fact]
    public void Owner_IsDaniel() => Assert.Equal("Daniel", _sut.Owner);

    [Fact]
    public void Model_ContainsTI() => Assert.Contains("TI", _sut.Model);

    [Fact]
    public void IsScientific_IsTrue() => Assert.True(_sut.IsScientific);

    [Fact]
    public void Add_TwoPlusThree_ReturnsFive() => Assert.Equal(5, _sut.Add(2, 3));

    [Theory]
    [InlineData("Daniel", true)]
    [InlineData("StudentCalc", false)]
    [InlineData("TI-84 Plus", true)]
    public void IsScientificFlagMatches(string owner, bool expectedScientific)
    {
        // Arrange
        var sut = new AdvancedCalculator
        {
            Owner = owner,
            IsScientific = expectedScientific
        };

        // Act
        var actual = sut.IsScientific;

        // Assert
        Assert.Equal(expectedScientific, actual);
    }

    [Theory]
    [InlineData("TI84 Plus", "TI")]
    [InlineData("Casio FX-991", "C")]
    [InlineData("Sharp EL-W531", "Sha")]
    public void Model_StartsWithExpectedPrefix(string model, string expectedPrefix)
    {
        // Arrange
        var sut = new AdvancedCalculator
        {
            Owner = "Daniel",
            Model = model
        };

        // Act
        var actual = sut.Model;

        // Assert
        Assert.StartsWith(expectedPrefix, actual);
    }
}
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
    public void Owner_IsDaniel() => Assert.Equal("Emmanuel", _sut.Owner);

    [Fact]
    public void Model_ContainsTI() => Assert.Contains("TI", _sut.Model);

    [Fact]
    public void IsScientific_IsTrue() => Assert.True(_sut.IsScientific);

    [Fact]
    public void Add_TwoPlusThree_ReturnsFive() => Assert.Equal(5, _sut.Add(2, 3));
}
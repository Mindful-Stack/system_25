namespace CalculatorApp.Tests;

public class AdvancedCalculatorFixture
{
    public AdvancedCalculator Calculator { get; }

    public AdvancedCalculatorFixture()
    {
        Calculator = new AdvancedCalculator
        {
            Owner = "Daniel",
            Model = "TI-84 Plus",
            IsScientific = true
        };
    }
}
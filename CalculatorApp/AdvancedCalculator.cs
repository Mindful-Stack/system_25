namespace CalculatorApp;

public class AdvancedCalculator
{
    public required string Owner { get; init; }
    public string Model { get; set; }
    public bool IsScientific { get; set; }

    public int Add(int a, int b) => a + b;
}
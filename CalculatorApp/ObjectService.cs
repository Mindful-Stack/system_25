namespace CalculatorApp;

public class ObjectService
{
    public object? GetNull() => null;

    public object GetSame() => new object();

    public void Crash() => throw new InvalidOperationException();
}

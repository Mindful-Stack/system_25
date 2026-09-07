namespace CalculatorApp;

public class CollectionService
{
    public List<string> GetItems() => new() { "one", "two", "three" };

    public List<string> GetEmpty() => new();
}

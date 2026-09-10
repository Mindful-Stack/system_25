namespace IceCreamMachine;

public class IceCreamMachine
{
    private readonly Dictionary<string, int> scoopTable = new()
    {
        { "S", 1 },
        { "M", 3 },
        { "L", 4 },
        { "XL", 5 }
    };
    
    public int GetScoops(string size)
    {
        return scoopTable.TryGetValue(size, out int scoops) ? scoops : 0;
    }

    public int GetPrice(string size)
    {
        var scoops = GetScoops(size);
        return scoops * 10;
    }

    public bool IsValidSize(string size)
    {
        return scoopTable.ContainsKey(size);
    }
}
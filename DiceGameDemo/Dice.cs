namespace DiceGameDemo;

// The real dice. Indeterministic, which is exactly why we want to replace it in tests.
// Everything is virtual so we can also mock the class itself (CallBase, slide 17).
public class Dice : IDice
{
    private readonly Random _random = new();

    public Dice() : this(6)
    {
    }

    public Dice(int sides) => Sides = sides;

    public virtual int Sides { get; }

    public virtual int Roll() => _random.Next(1, Sides + 1);

    public virtual string Describe() => $"a {Sides}-sided dice";
}

namespace DiceGameDemo;

// The boring real implementations, only used by Program.cs.
// In the tests we never touch these.

public class ConsoleNotifier : INotifier
{
    public void Notify(string message) => Console.WriteLine(message);
}

public class InMemoryScoreBoard : IScoreBoard
{
    private readonly Dictionary<string, int> _scores = [];

    public int GetHighScore() => _scores.Count == 0 ? 0 : _scores.Values.Max();

    public Task<bool> SaveScoreAsync(string playerName, int score)
    {
        _scores[playerName] = score;
        return Task.FromResult(true);
    }
}

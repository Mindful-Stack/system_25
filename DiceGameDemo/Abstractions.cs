namespace DiceGameDemo;

// Everything the game depends on, behind interfaces.
// That is the whole trick: the game never knows it is talking to a mock.

public interface IDice
{
    int Sides { get; }

    int Roll();
}

public interface IPlayer
{
    string Name { get; set; }

    int Score { get; set; }
}

public interface IScoreBoard
{
    int GetHighScore();

    Task<bool> SaveScoreAsync(string playerName, int score);
}

public interface INotifier
{
    void Notify(string message);
}

public class ScoreBoardUnavailableException(string message) : Exception(message);

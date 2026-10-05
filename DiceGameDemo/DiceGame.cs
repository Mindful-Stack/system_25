namespace DiceGameDemo;

// The system under test. Rolls one dice per round:
// a 1 wipes your score, anything else is added to it.
public class DiceGame
{
    private readonly IDice _dice;
    private readonly IScoreBoard _scoreBoard;
    private readonly INotifier _notifier;

    public DiceGame(IDice dice, IScoreBoard scoreBoard, INotifier notifier)
    {
        _dice = dice;
        _scoreBoard = scoreBoard;
        _notifier = notifier;
    }

    public int PlayRound(IPlayer player)
    {
        var roll = _dice.Roll();

        if (roll == 1)
        {
            player.Score = 0;
            _notifier.Notify($"{player.Name} rolled a 1 and lost everything!");
            return 0;
        }

        player.Score += roll;
        _notifier.Notify($"{player.Name} scored {roll} points.");
        return roll;
    }

    public async Task<bool> FinishGameAsync(IPlayer player)
    {
        var highScore = GetHighScoreOrZero();
        var saved = await _scoreBoard.SaveScoreAsync(player.Name, player.Score);

        _notifier.Notify(player.Score > highScore
            ? $"New high score for {player.Name}: {player.Score}!"
            : $"{player.Name} finished with {player.Score} points.");

        return saved;
    }

    public void Rename(IPlayer player, string newName)
    {
        _notifier.Notify($"{player.Name} is now called {newName}.");
        player.Name = newName;
    }

    private int GetHighScoreOrZero()
    {
        try
        {
            return _scoreBoard.GetHighScore();
        }
        catch (ScoreBoardUnavailableException)
        {
            _notifier.Notify("Scoreboard is unavailable, playing offline.");
            return 0;
        }
    }
}

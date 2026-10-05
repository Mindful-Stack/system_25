namespace DiceGameDemo;

// A real player object. Simple enough to use straight in tests as a fake,
// which is often better than mocking it (slide 5: state verification).
public class Player : IPlayer
{
    public string Name { get; set; } = "Player 1";

    public int Score { get; set; }
}

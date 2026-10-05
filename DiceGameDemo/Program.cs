using DiceGameDemo;

var dice = new Dice();
var game = new DiceGame(dice, new InMemoryScoreBoard(), new ConsoleNotifier());
var player = new Player { Name = "Daniel" };

Console.WriteLine($"Rolling {dice.Describe()} until we hit a 1...");

while (game.PlayRound(player) > 0)
{
}

await game.FinishGameAsync(player);

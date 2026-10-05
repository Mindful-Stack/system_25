namespace DiceGameDemo.Tests;

// DEMO 1 - slides 11-14
// Creating mocks, configuring them, matching arguments, verifying calls.
public class Demo1_TheBasicsTests
{
   // This isn't really a test, we just show how to use Moq
   [Fact]
   public void CreateOurFirstMock()
   {
      // Arrange 
      var mockDice = new Mock<IDice>();
      mockDice.Setup(x => x.Sides).Returns(6);
      mockDice.Setup(x => x.Roll()).Returns(4);
      
      var dice = mockDice.Object;

      // Act
      var actualRoll = dice.Roll();
      var secondRoll = dice.Roll();
      var actualSides = dice.Sides;
      
      // Assert
      Assert.Equal(4, actualRoll);
      Assert.Equal(4, secondRoll);
      Assert.Equal(6, actualSides);
   }

   [Fact]
   public void PlayRound_AddsTheRollToTheScore()
   {
      // Arrange
      var mockDice = new Mock<IDice>();
      mockDice.Setup(x => x.Roll()).Returns(4);
      
      var mockScoreBoard = new Mock<IScoreBoard>();
      var mockNotifier = new Mock<INotifier>();

      var game = new DiceGame(
         mockDice.Object, 
         mockScoreBoard.Object, 
         mockNotifier.Object);

      var mockPlayer = new Mock<IPlayer>();
      mockPlayer.SetupProperty(x => x.Score);
      var player = mockPlayer.Object;
      // Act
      var points = game.PlayRound(player);
      
      // Assert
      Assert.Equal(4, points);
      Assert.Equal(4, player.Score);
   }
   
   [Fact]
   public void PlayRound_WipesTheScoreWhenWeRollAOne()
   {
      // Arrange
      var mockDice = new Mock<IDice>();
      mockDice.Setup(x => x.Roll()).Returns(1);
      
      var mockScoreBoard = new Mock<IScoreBoard>();
      var mockNotifier = new Mock<INotifier>();

      var sut = new DiceGame(
         mockDice.Object, 
         mockScoreBoard.Object, 
         mockNotifier.Object);

      var mockPlayer = new Mock<IPlayer>();
      mockPlayer.SetupProperty(x => x.Score);
      var player = mockPlayer.Object;
      player.Score = 42;
      
      // Act
      var points = sut.PlayRound(player);
      
      // Assert
      Assert.Equal(0, points);
      Assert.Equal(0, player.Score);
   }

   [Fact]
   public async Task CanConfigureAsyncMethod()
   {
      // Arrange
      var mockDice = new Mock<IDice>();
      mockDice.Setup(x => x.Roll()).Returns(1);
      
      var mockScoreBoard = new Mock<IScoreBoard>();
      mockScoreBoard.Setup(x => 
         x.SaveScoreAsync(It.IsAny<string>(), 0)).ReturnsAsync(true);
      
      var mockNotifier = new Mock<INotifier>();

      var sut = new DiceGame(
         mockDice.Object, 
         mockScoreBoard.Object, 
         mockNotifier.Object);

      var mockPlayer = new Mock<IPlayer>();
      mockPlayer.SetupProperty(x => x.Score);
      var player = mockPlayer.Object;

      
      // Act
      var actual = await sut.FinishGameAsync(player);
      
      // Assert
      Assert.True(actual);
   }
   
}

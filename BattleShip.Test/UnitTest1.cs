using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using BattleShip.Controller;
using BattleShip.Interfaces;


[TestFixture]
public class GameControllerTests
{
    [Test]
    public void SetCurrentPlayer_WhenPlayerExists_ShouldSetCurrentPlayerAndReturnTrue()
    {
        // Arrange
        var playerMock = new Mock<IPlayer>();
        var gameController = new GameController();
        gameController.Players = new List<IPlayer> { playerMock.Object };

        // Act
        var result = gameController.SetCurrentPlayer(playerMock.Object);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(playerMock.Object, gameController.CurrentPlayer);
    }

    [Test]
    public void SetCurrentPlayer_WhenPlayerDoesNotExist_ShouldNotSetCurrentPlayerAndReturnFalse()
    {
        // Arrange
        var playerMock = new Mock<IPlayer>();
        var anotherPlayerMock = new Mock<IPlayer>();
        var gameController = new GameController();
        gameController.Players = new List<IPlayer> { playerMock.Object };

        // Act
        var result = gameController.SetCurrentPlayer(anotherPlayerMock.Object);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(gameController.CurrentPlayer);
    }
}

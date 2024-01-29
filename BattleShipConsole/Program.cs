using BattleShip.Controller;
using BattleShip.Enums;
using BattleShip.UI;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

internal class Program {
    private static void Main(string[] args) {

        //manual creation w/o dependency injection
        var loggerFactory = LoggerFactory.Create(b =>
                    {
                        b.ClearProviders();
                        b.SetMinimumLevel(LogLevel.Information);
                        b.AddNLog("nlog.config");
                    });
        ILogger<GameController> logger = loggerFactory.CreateLogger<GameController>();

        GameController game = new GameController(logger); 

        while (true)
        {
            if (game.Status == GameStatus.NotReady)
            {
                Menus.StartMenu(game);
            }
            else if (game.Status == GameStatus.InitializeGame)
            {
                Menus.InitializeGameMenu(game);
            }
            else if (game.Status == GameStatus.InProgress)
            {
                Menus.GamePlayMenu(game);
            }
            else if (game.Status == GameStatus.GameEnd)
            {
                Menus.ExitMenu(game);
            }
        }
    }
}
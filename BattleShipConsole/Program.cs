using BattleShip.Controller;
using BattleShip.Enums;
using BattleShip.UI;

internal class Program {
    private static void Main(string[] args) {
        GameController gameController = new();

        while (true)
        {
            if (gameController.Status == GameStatus.NotReady)
            {
                Menus.StartMenu(gameController);
            }
            else if (gameController.Status == GameStatus.InitializeGame)
            {
                Menus.InitializeGameMenu(gameController);
            }
            else if (gameController.Status == GameStatus.InProgress)
            {
                Menus.GamePlayMenu(gameController);
            }
            else if (gameController.Status == GameStatus.GameEnd)
            {
                Menus.ExitMenu(gameController);
            }
        }
    }
}
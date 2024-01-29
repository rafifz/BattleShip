using BattleShip.Controller;
using BattleShip.Enums;


namespace BattleShip.UI;
public class Menus
{
    public static void StartMenu(GameController gameController)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8; // set the console output encoding to UTF-8

        string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "UI");
        string filePath = Path.Combine(basePath, "Ascii.txt");

        using (StreamReader reader = new StreamReader(File.Open(filePath, FileMode.Open)))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine()!;
                Console.WriteLine(@$"{line}");
            }
        }
        gameController.Players.Clear();
        Console.WriteLine("\n\n Press anything to start..");
        Console.ReadKey();
        Console.Clear();

        gameController.UpdateGameStatus(); //game init
    }


    public static void InitializeGameMenu(GameController gameController)
    {
        GameUI.AddPlayerInput(gameController);
        Console.Clear();

        gameController.UpdateGameStatus(); //game ready
        gameController.UpdateGameStatus(); //game in progress
    }

    public static void GamePlayMenu(GameController gameController)
    {
        while (gameController.Status != GameStatus.GameEnd)
        {
            Console.WriteLine($"TURN  {gameController.NumberOfTurn} : {gameController.CurrentPlayer?.Name}'s Turn ");

            GameUI.PrintBoard(gameController.PlayerBattleBoard[gameController.CurrentPlayer!]);
            GameUI.PrintBoard(gameController.PlayerMainBoard[gameController.CurrentPlayer!]);
            GameUI.PrintOpponentShip(gameController);
            GameUI.OnGameInput(gameController);
            Console.Clear();
            if (gameController.Status != GameStatus.GameEnd)
            {
                gameController.NextTurn();
            }
        }
    }
    public static void ExitMenu(GameController gameController)
    {
        Console.WriteLine($"{gameController.CurrentPlayer?.Name} HAS WON THE BATTLE.");
        Console.Write("press any key to continue..");
        Console.ReadKey();
        Console.Clear();

        GameUI.OnExitInput(gameController);
    }
}
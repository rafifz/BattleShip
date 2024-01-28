using BattleShip.Controller;
using BattleShip.Enums;
using BattleShip.Interfaces;
using BattleShip.Models;

namespace BattleShip.UI;
public class GameUI
{
    public static void PrintBoard(IBoard board)
    {
        Console.WriteLine($"\n\t\t\t-- {board.GetType().Name} --\n");
        // Print rows
        for (int i = 0; i < board.Rows; i++)
        {
            Console.Write($"{i + 1,2} ");

            // Print columns
            for (int j = 0; j < board.Columns; j++)
            {
                IPosition currentPos = board.BoardPositions[i * board.Columns + j];
                PrintCellState(currentPos.State);
            }
            Console.WriteLine();
        }
        // Print column numbers
        for (int i = 1; i <= board.Columns; i++)
        {
            Console.Write($"{i,6} ");
        }
        Console.WriteLine();
    }

    private static void PrintCellState(CellState state)
    {
        switch (state)
        {
            case CellState.Empty:
                Console.Write("|    | ");
                break;
            case CellState.Miss:
                Console.Write("|MISS| ");
                break;
            case CellState.Hit:
                Console.Write("|HIT!| ");
                break;
            case CellState.Ship:
                Console.Write("|SHIP| ");
                break;
            case CellState.Sink:
                Console.Write("|xxxx| ");
                break;
            default:
                Console.Write("|????| ");
                break;
        }
    }

    public static void PrintOpponentShip(GameController gameController)
    {
        var playerOwnedShips = gameController.PlayerMainBoard[gameController.Opponent!].OwnedShips;
        Console.WriteLine($"\n---------- {gameController.Opponent.Name}'s ships : ----------");
        foreach (var ship in playerOwnedShips)
        {
            Console.WriteLine($"{ship.Type} {ship.LifePoint} IsDestroyed: {ship.IsDestroyed}");
        }
        Console.WriteLine("----------------------------------");
    }
    public static void PrintPlayerUnmanagedShips(GameController gameController, IPlayer player)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        int count = 1;

        System.Console.WriteLine();
        foreach (var shipType in gameController.PlayersUnmanagedShips[player])
        {
            int size = shipType switch
            {
                ShipType.AircraftCarrier => 5,
                ShipType.Battleship => 4,
                ShipType.Cruiser => 3,
                ShipType.Submarine => 3,
                ShipType.Destroyer => 2,
                _ => throw new ArgumentException($"Unknown ship type: {shipType}"),
            };

            string shipShape = new string('☒', size);
            Console.WriteLine($"[{count}] {shipType} ");
            count++;
        }
        System.Console.WriteLine();
    }

    public static void AddPlayerInput(GameController gameController)
    {
        int numberOfPlayer = gameController.Players.Count;
        while (gameController.Players.Count < 2)
        {
            Console.Write($"Enter player {++numberOfPlayer} name: ");
            string playerName = Console.ReadLine()!;

            IPlayer player = new Player(gameController.Players.Count + 1, playerName);
            AddPlayer(player, gameController);
            Console.WriteLine($"Player '{playerName}' added successfully!");
            Thread.Sleep(500);
            Console.Clear();

            ShipConfigurationInput(gameController, player);
        }
        gameController.SetCurrentPlayer(gameController.Players[new Random().Next(gameController.Players.Count)]);
        gameController.NextTurn();
    }
    public static bool AddPlayer(IPlayer player, GameController gameController)
    {
        gameController.Players.Add(player);
        gameController.PlayerMainBoard.Add(player, new MainBoard(8, 8));
        gameController.PlayerBattleBoard.Add(player, new BattleBoard(8, 8));
        gameController.PlayersUnmanagedShips[player] = Enum.GetValues(typeof(ShipType)).Cast<ShipType>().ToList();
        return true;
    }
    public static void ShipConfigurationInput(GameController gameController, IPlayer player)
    {
        Console.WriteLine("\nWelcome to the Ship Configuration Console:");
        Console.WriteLine("\n[1] Set ship manually\n[2] Set ship randomly");
        Console.Write("\nPlease enter the number corresponding to your choice: ");
        string userInput = Console.ReadLine()!.ToLower();

        if (userInput == "1")
        {
            Console.Clear();
            PrintBoard(gameController.PlayerMainBoard[player]);
            PrintPlayerUnmanagedShips(gameController, player);
            while (gameController.PlayersUnmanagedShips[player].Count > 0)
            {
                ChoseUnmanagedShipInput(gameController, player);
            }
        }
        else if (userInput == "2")
        {
            // Ship ship = new (new Position(1,1), ShipType.Destroyer, ShipOrientation.Horizontal);
            // gameController.TrySetShip(gameController.PlayerMainBoard[player], ship);
            gameController.SetRandomShip(gameController.PlayerMainBoard[player]);
            Console.WriteLine("Ship successfully set randomly");
            Thread.Sleep(500);
            Console.Clear();
        }
        else
        {
            Console.Clear();
            Console.WriteLine("Invalid input. Please try again.");
            ShipConfigurationInput(gameController, player);
        }
    }

    public static void ChoseUnmanagedShipInput(GameController gameController, IPlayer player)
    {
        Console.Clear();
        PrintBoard(gameController.PlayerMainBoard[player]);
        PrintPlayerUnmanagedShips(gameController, player);

        while (gameController.PlayersUnmanagedShips[player].Count > 0)
        {
            Console.Clear();
            PrintBoard(gameController.PlayerMainBoard[player]);
            PrintPlayerUnmanagedShips(gameController, player);

            Console.Write("Please enter the number corresponding to the ship you want to set: ");
            string userInputShip = Console.ReadLine()!.ToLower();

            if (int.TryParse(userInputShip, out int shipIndex) && shipIndex >= 1 &&
                shipIndex <= gameController.PlayersUnmanagedShips[player].Count)
            {
                SetShipWithUserInput(gameController, player, shipIndex);
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid ship number.");
                Thread.Sleep(700);
            }
        }
        Console.Write("All ships have been placed, press any key to continue..");
        Console.ReadKey();
        Console.Clear();
    }

    public static void SetShipWithUserInput(GameController gameController, IPlayer player, int shipIndex)
    {
        Console.Write("\nEnter your orientation and position (e.g., 'Horizontal/Vertical x, y'): ");
        string userInputShipPlacement = Console.ReadLine()!.ToLower();

        ShipOrientation orientation;
        int x, y;

        if (TryParseOrientationAndCoordinates(userInputShipPlacement, out orientation, out x, out y))
        {
            Ship ship = new Ship(new Position(x, y), gameController.PlayersUnmanagedShips[player][shipIndex - 1],
                orientation);

            if (gameController.TrySetShip(gameController.PlayerMainBoard[player], ship))
            {
                gameController.PlayersUnmanagedShips[player].RemoveAt(shipIndex - 1);
            }
            else
            {
                Console.WriteLine("Ship out of board range.");
                SetShipWithUserInput(gameController, player, shipIndex);
            }
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid orientation and position.");
            SetShipWithUserInput(gameController, player, shipIndex);
        }
    }

    public static void OnGameInput(GameController gameController)
    {
        Console.Write("\nEnter your move (e.g., 'attack x, y'): ");
        string userInput = Console.ReadLine()!.ToLower();

        if (userInput.StartsWith("attack") && TryParseAttackCoordinates(userInput, out int x, out int y))
        {
            OnAttackInput(gameController, x, y);
        }
        else
        {
            Console.WriteLine("Invalid input. Please try again.");
            OnGameInput(gameController);
        }
    }

    public static void OnAttackInput(GameController gameController, int x, int y)
    {
        IPosition attackPosition = gameController.PlayerBattleBoard[gameController.CurrentPlayer!].BoardPositions.FirstOrDefault(p => p.X == x && p.Y == y)!;

        if (attackPosition != null && attackPosition.State == CellState.Empty)
        {
            gameController.PerformAttack(attackPosition);
            Console.Clear();

            Console.WriteLine($"TURN {gameController.NumberOfTurn}: {gameController.CurrentPlayer?.Name}'s Turn");
            PrintBoard(gameController.PlayerBattleBoard[gameController.CurrentPlayer!]);
            PrintBoard(gameController.PlayerMainBoard[gameController.CurrentPlayer!]);
            PrintOpponentShip(gameController);

            if (gameController.Status == GameStatus.GameEnd)
            {
                Console.WriteLine($"All {gameController.Opponent} ship's have been destroyed");
                Thread.Sleep(2000);
            }
            else if (attackPosition != null && attackPosition.State == CellState.Hit)
            {
                Console.WriteLine("\nTarget Hit");
                OnGameInput(gameController);
            }
            else
            {
                Console.WriteLine("\nTarget Miss");
                Console.Write("Press space to end turn");
                Console.ReadKey();
            }
        }
        else if (attackPosition != null && attackPosition.State != CellState.Empty)
        {
            Console.WriteLine("Coordinates already attacked. Please try again.");
            OnGameInput(gameController);
        }
        else
        {
            Console.WriteLine("Invalid coordinates. Please try again.");
            OnGameInput(gameController);
        }
    }

    public static void OnExitInput(GameController gameController)
    {
        Console.WriteLine("[1] Exit Game");
        Console.WriteLine("[2] Play Again");
        while (true)
        {
            Console.Write("\nEnter your input: ");
            string userInput = Console.ReadLine()!;
            if (userInput == "1")
            {
                Environment.Exit(0);
            }
            else if (userInput == "2")
            {
                Console.Clear();
                gameController.SetGameStatus(GameStatus.InitializeGame);
                gameController.Players.Clear();
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter either '1' or '2'.");
            }
        }
    }

    public static bool TryParseAttackCoordinates(string input, out int x, out int y)
    {
        x = y = 0;

        string[] coordinates = input.Split(' ');

        if (coordinates.Length >= 2)
        {
            string[] values = coordinates[1].Split(',');
            if (values.Length == 2 && int.TryParse(values[0], out y) && int.TryParse(values[1], out x))
            {
                return true;
            }
        }

        return false;
    }
    public static bool TryParseShipCoordinates(string input, out int x, out int y)
    {
        x = y = 0;

        string[] coordinates = input.Split(',');

        if (coordinates.Length == 2 && int.TryParse(coordinates[0], out y) && int.TryParse(coordinates[1], out x))
        {
            return true;
        }

        return false;
    }

    public static bool TryParseOrientationAndCoordinates(string input, out ShipOrientation orientation, out int x,
        out int y)
    {
        orientation = ShipOrientation.Horizontal;
        x = y = 0;

        string[] parts = input.Split(' ');

        if (parts.Length == 2)
        {
            orientation = parts[0] == "vertical" ? ShipOrientation.Horizontal : ShipOrientation.Vertical;

            if (TryParseShipCoordinates(parts[1], out x, out y))
            {
                return true;
            }
        }

        return false;
    }
}

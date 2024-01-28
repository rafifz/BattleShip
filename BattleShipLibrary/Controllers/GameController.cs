using BattleShip.Enums;
using BattleShip.Interfaces;
using BattleShip.Models;

namespace BattleShip.Controller
{
    /// <summary>
    /// The game controller for the battleship game.
    /// </summary>
    public class GameController
    {
        /// <summary>
        /// Gets or sets the current player.
        /// </summary>
        public IPlayer? CurrentPlayer { get; private set; }

        /// <summary>
        /// Gets the number of turns.
        /// </summary>
        public int NumberOfTurn { get; private set; }

        /// <summary>
        /// Gets the list of players.
        /// </summary>
        public List<IPlayer> Players { get; private set; }

        /// <summary>
        /// Gets the main board for each player.
        /// </summary>
        public Dictionary<IPlayer, MainBoard> PlayerMainBoard { get; private set; }

        /// <summary>
        /// Gets the battle board for each player.
        /// </summary>
        public Dictionary<IPlayer, BattleBoard> PlayerBattleBoard { get; private set; }

        /// <summary>
        /// Gets the list of unmanaged ships for each player.
        /// </summary>
        public Dictionary<IPlayer, List<ShipType>> PlayersUnmanagedShips { get; private set; }

        /// <summary>
        /// Gets or sets the current game status.
        /// </summary>
        public GameStatus Status { get; set; }

        /// <summary>
        /// Gets the index of the next player.
        /// </summary>
        public int NextPlayerIndex => (Players.IndexOf(CurrentPlayer!) + 1) % Players.Count;

        /// <summary>
        /// Gets the opponent of the current player.
        /// </summary>
        public IPlayer Opponent => Players[NextPlayerIndex];

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        public GameController()
        {
            Players = new List<IPlayer>();
            PlayerMainBoard = new Dictionary<IPlayer, MainBoard>();
            PlayerBattleBoard = new Dictionary<IPlayer, BattleBoard>();
            PlayersUnmanagedShips = new Dictionary<IPlayer, List<ShipType>>();
        }

        /// <summary>
        /// Sets the current player.
        /// </summary>
        /// <param name="player">The player to set as the current player.</param>
        /// <returns>True if the specified player is in the list of players and is successfully set as the current player, false otherwise.</returns>
        public bool SetCurrentPlayer(IPlayer player)
        {
            if (Players.Contains(player))
            {
                CurrentPlayer = player;
                return true; // Successfully set the current player
            }

            return false; // Player not found in the list
        }

        /// <summary>
        /// Set the current game status.
        /// </summary>
        /// <param name="newStatus">The game status to set.</param>
        public void SetGameStatus(GameStatus newStatus)
        {
            Status = newStatus;
        }

        /// <summary>
        /// Updates the game status.
        /// </summary>
        /// <returns>The updated game status.</returns>
        public GameStatus UpdateGameStatus()
        {
            switch (Status)
            {
                case GameStatus.NotReady:
                    Status = GameStatus.InitializeGame;
                    break;
                case GameStatus.InitializeGame:
                    Status = GameStatus.Ready;
                    break;
                case GameStatus.Ready:
                    Status = GameStatus.InProgress;
                    break;
                case GameStatus.InProgress:
                    Status = GameStatus.GameEnd;
                    break;
                default:
                    throw new InvalidOperationException("Invalid game status value.");
            }
            return Status;
        }

        /// <summary>
        /// Advances to the next turn.
        /// </summary>
        public void NextTurn()
        {
            NumberOfTurn++;
            CurrentPlayer = Players[NextPlayerIndex];
        }

        /// <summary>
        /// Checks if the win condition has been met.
        /// </summary>
        /// <returns>True if all the opponent ownedship IsDestroyed, false otherwise.</returns>
        private bool CheckWinCondition()
        {
            if (PlayerMainBoard[Opponent].OwnedShips.All(ship => ship.IsDestroyed))
            {
                Status = GameStatus.GameEnd;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sets a random ship on the main board.
        /// </summary>
        /// <param name="mainBoard">The main board to set the ship on.</param>
        public void SetRandomShip(MainBoard mainBoard)
        {
            Random random = new Random();

            // create list of ships based on ShipType enum values
            List<ShipType> shipTypes = Enum.GetValues(typeof(ShipType)).Cast<ShipType>().ToList();

            foreach (ShipType shipType in shipTypes)
            {
                // Generate random start position and orientation
                int startX = random.Next(1, mainBoard.Rows + 1);
                int startY = random.Next(1, mainBoard.Columns + 1);
                ShipOrientation orientation = (ShipOrientation)random.Next(2); // 0 for Vertical, 1 for Horizontal

                // Create ship with the correct length
                Ship ship = new Ship(new Position(startX, startY), shipType, orientation);

                while (!TrySetShip(mainBoard, ship)) // Attempt to set the ship on the board, retry if it fails
                {
                    startX = random.Next(1, mainBoard.Rows + 1);
                    startY = random.Next(1, mainBoard.Columns + 1);
                    orientation = (ShipOrientation)random.Next(2);

                    ship = new Ship(new Position(startX, startY), shipType, orientation);
                }
            }
        }

        /// <summary>
        /// Attempts to set a ship on the main board.
        /// </summary>
        /// <param name="mainBoard">The main board to set the ship on.</param>
        /// <param name="ship">The ship to set.</param>
        /// <returns>True if the ship was successfully set, false otherwise.</returns>
        public bool TrySetShip(MainBoard mainBoard, Ship ship)
        {
            int startX = ship.Start.X;
            int startY = ship.Start.Y;
            int length = ship.Length;

            if (ship.Orientation == ShipOrientation.Vertical)
            {
                if (!TryPlaceShip(mainBoard, ship, startX, startY, 0, 1, length))
                {
                    return false;
                }
            }
            else if (ship.Orientation == ShipOrientation.Horizontal)
            {
                if (!TryPlaceShip(mainBoard, ship, startX, startY, 1, 0, length))
                {
                    return false;
                }
            }

            mainBoard.OwnedShips.Add(ship);
            return true;
        }

        /// <summary>
        /// Attempts to place a ship on the main board.
        /// </summary>
        /// <param name="mainBoard">The main board to place the ship on.</param>
        /// <param name="ship">The ship to place.</param>
        /// <param name="startX">The starting X position of the ship.</param>
        /// <param name="startY">The starting Y position of the ship.</param>
        /// <param name="offsetX">The X offset to use when placing the ship horizontally.</param>
        /// <param name="offsetY">The Y offset to use when placing the ship vertically.</param>
        /// <param name="length">The length of the ship.</param>
        /// <returns>True if the ship was successfully placed, false otherwise.</returns>
        private bool TryPlaceShip(MainBoard mainBoard, Ship ship, int startX, int startY, int offsetX, int offsetY, int length)
        {
            if ( //ensure that does not go outside the bounds of the mainBoard.
                startX + offsetX * length > mainBoard.Columns + 1 ||
                startY + offsetY * length > mainBoard.Rows + 1
            )
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                int x = startX + i * offsetX;
                int y = startY + i * offsetY;

                IPosition shipPosition = mainBoard.BoardPositions.FirstOrDefault(p => p.X == x && p.Y == y)!;
                if (shipPosition == null || shipPosition.State != CellState.Empty) // return  false if the cell state is not empty
                {
                    return false;
                }

                shipPosition.State = CellState.Ship;
                ship.ShipPositions.Add(shipPosition);
            }

            return true;
        }

        /// <summary>
        /// Performs an attack on a given position.
        /// </summary>
        /// <param name="position">The position to attack.</param>
        /// <returns>True if the attack was successful, false otherwise.</returns>
        public bool PerformAttack(IPosition position)
        {
            IPosition ownPositionInBattleBoard = PlayerBattleBoard[CurrentPlayer!].BoardPositions.FirstOrDefault(p => p.X == position.X && p.Y == position.Y)!;
            IPosition ownPositionInMainBoard = PlayerMainBoard[CurrentPlayer!].BoardPositions.FirstOrDefault(p => p.X == position.X && p.Y == position.Y)!;
            IPosition enemyPositionInBattleBoard = PlayerBattleBoard[Opponent].BoardPositions.FirstOrDefault(p => p.X == position.X && p.Y == position.Y)!;
            IPosition enemyPositionInMainBoard = PlayerMainBoard[Opponent].BoardPositions.FirstOrDefault(p => p.X == position.X && p.Y == position.Y)!;

            var enemyOwnedShip = PlayerMainBoard[Opponent].OwnedShips;
            if (position != null && enemyPositionInMainBoard.State == CellState.Empty) //attack miss
            {
                ownPositionInBattleBoard.State = CellState.Miss;
            }
            else if (position != null && enemyPositionInMainBoard.State == CellState.Ship) //attack success
            {
                ownPositionInBattleBoard.State = CellState.Hit;
                enemyPositionInMainBoard.State = CellState.Sink;

                DecreaseShipsLife(PlayerMainBoard[Opponent].OwnedShips, position);
                return true;
            }
            return true;
        }

        /// <summary>
        /// Decreases the life of a ship.
        /// </summary>
        /// <param name="ownedShips">The list of owned ships.</param>
        /// <param name="position">The position of the ship to decrease the life of.</param>
        private void DecreaseShipsLife(List<Ship> ownedShips, IPosition position)
        {
            var affectedShip = ownedShips.FirstOrDefault(ship =>
                ship.ShipPositions.Any(pos => pos.X == position.X && pos.Y == position.Y)
            );

            if (affectedShip != null)
            {
                affectedShip.LifePoint--; // Decrement LifePoint of the ship

                if (affectedShip.LifePoint == 0) // If LifePoint becomes 0, the ship is destroyed
                {
                    affectedShip.IsDestroyed = true;
                    CheckWinCondition();
                }
            }
        }

    }
}

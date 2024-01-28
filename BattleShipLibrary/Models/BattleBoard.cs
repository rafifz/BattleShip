using BattleShip.Interfaces;

namespace BattleShip.Models;
public class BattleBoard : IBoard
{
    public int Rows
    { get; set; }
    public int Columns
    { get; set; }
    public List<IPosition> BoardPositions
    { get; set; }

    public BattleBoard(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        BoardPositions = new List<IPosition>();
        InitializeBoard();
    }

    public void InitializeBoard()
    {
        for (int i = 1; i < Rows + 1; i++)
        {
            for (int j = 1; j < Columns + 1; j++)
            {
                BoardPositions.Add(new Position(i, j));
            }
        }
    }
}
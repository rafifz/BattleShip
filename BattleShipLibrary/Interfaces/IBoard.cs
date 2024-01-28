namespace BattleShip.Interfaces;
public interface IBoard
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public List<IPosition> BoardPositions { get; set; }

    public void InitializeBoard();
}

using BattleShip.Enums;

namespace BattleShip.Interfaces;
public interface IPosition
{
     public int X { get; set; }
    public int Y { get; set; }
    public CellState State { get; set; }
}
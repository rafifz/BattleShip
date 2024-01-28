using BattleShip.Enums;
using BattleShip.Interfaces;

namespace BattleShip.Models;
public class Position : IPosition
{
    public int X { get; set; }
    public int Y { get; set; }
    public CellState State { get; set; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
        State = CellState.Empty; // Set the default state to Empty
    }
}
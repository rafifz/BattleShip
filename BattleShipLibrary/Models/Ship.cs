using BattleShip.Enums;
using BattleShip.Interfaces;

namespace BattleShip.Models;
public class Ship : IShip
{
    public int Id { get; set; }
    public bool IsDestroyed { get; set; }
    public IPosition Start { get; set; }
    public List<IPosition> ShipPositions { get; set; }
    public ShipType Type { get; set; }
    public ShipOrientation Orientation { get; set; }
    public int Length { get; private set; }
    public int LifePoint { get; set; }

    public Ship(IPosition start, ShipType type, ShipOrientation orientation)
    {
        Start = start;
        ShipPositions = new List<IPosition>();
        Type = type;
        Orientation = orientation;
        SetShipLength();
        SetShipLifePoint();
    }
    public int SetShipLength()
    {
        switch (Type)
        {
            case ShipType.AircraftCarrier:
                return Length = 5;
            case ShipType.Battleship:
                return Length = 4;
            case ShipType.Cruiser:
                return Length = 3;
            case ShipType.Submarine:
                return Length = 3;
            case ShipType.Destroyer:
                return Length = 2;
            default:
                throw new ArgumentOutOfRangeException(nameof(Type), Type, "Invalid ship type");
        }
    }
    public int SetShipLifePoint()
    {
        return LifePoint = Length;
    }
}

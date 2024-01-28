using BattleShip.Enums;

namespace BattleShip.Interfaces;
public interface IShip
{
    public int Id { get; set; }
    public bool IsDestroyed { get; set; }
    public IPosition Start { get; set; }
    public List<IPosition> ShipPositions { get; set; }
    public ShipType Type { get; set; }
    public ShipOrientation Orientation { get; set; }
}

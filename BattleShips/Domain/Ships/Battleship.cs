namespace BattleShips.Domain.Ships;

public sealed class Battleship(BattleShips.Domain.Position start, BattleShips.Domain.Orientation o)
    : ShipBase(start, o)
{
    public override string Name => "Battleship";
    public override int Length => 4;
}
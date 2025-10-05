namespace BattleShips.Domain.Ships;

public sealed class Submarine(BattleShips.Domain.Position start, BattleShips.Domain.Orientation o)
    : ShipBase(start, o)
{
    public override string Name => "Submarine";
    public override int Length => 3;
}
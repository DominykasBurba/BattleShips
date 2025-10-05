namespace BattleShips.Domain.Ships;

public sealed class Destroyer(BattleShips.Domain.Position start, BattleShips.Domain.Orientation o)
    : ShipBase(start, o)
{
    public override string Name => "Destroyer";
    public override int Length => 2;
}
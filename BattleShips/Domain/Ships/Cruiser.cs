namespace BattleShips.Domain.Ships;

public sealed class Cruiser(BattleShips.Domain.Position start, BattleShips.Domain.Orientation o)
    : ShipBase(start, o)
{
    public override string Name => "Cruiser";
    public override int Length => 1;
}
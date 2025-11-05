namespace BattleShips.Domain.Ships.Modern;

/// <summary>
/// Modern variant of Cruiser with missile systems.
/// Part of the Modern ship family created by ModernShipFactory.
/// </summary>
public sealed class ModernCruiser(Position start, Orientation o) : ShipBase(start, o)
{
    public override string Name => "Modern Cruiser";
    public override int Length => 2; // Enhanced: Longer than classic (1)
}

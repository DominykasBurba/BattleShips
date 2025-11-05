namespace BattleShips.Domain.Ships.Modern;

/// <summary>
/// Modern variant of Destroyer with advanced weaponry.
/// Part of the Modern ship family created by ModernShipFactory.
/// </summary>
public sealed class ModernDestroyer(Position start, Orientation o) : ShipBase(start, o)
{
    public override string Name => "Modern Destroyer";
    public override int Length => 3; // Enhanced: Longer than classic (2)
}

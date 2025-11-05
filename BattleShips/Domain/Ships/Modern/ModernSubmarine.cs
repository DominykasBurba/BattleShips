namespace BattleShips.Domain.Ships.Modern;

/// <summary>
/// Modern variant of Submarine with stealth technology.
/// Part of the Modern ship family created by ModernShipFactory.
/// </summary>
public sealed class ModernSubmarine(Position start, Orientation o) : ShipBase(start, o)
{
    public override string Name => "Modern Submarine";
    public override int Length => 4; // Enhanced: Longer than classic (3)
}

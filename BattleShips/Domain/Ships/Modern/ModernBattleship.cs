namespace BattleShips.Domain.Ships.Modern;

/// <summary>
/// Modern variant of Battleship with enhanced capabilities.
/// Part of the Modern ship family created by ModernShipFactory.
/// </summary>
public sealed class ModernBattleship(Position start, Orientation o) : ShipBase(start, o)
{
    public override string Name => "Modern Battleship";
    public override int Length => 5; // Enhanced: Longer than classic (4)
}

namespace BattleShips.Domain.Ships.Decorators;

/// <summary>
/// Concrete Decorator that applies a skin to a ship.
/// Demonstrates the Decorator pattern by:
/// - Changing visual appearance (skin)
/// - Adding behavior: Camouflage skin provides +1 extra hit point
/// </summary>
public class SkinnedShipDecorator(IShip ship, ShipSkin skin) : ShipDecorator(ship)
{
    private bool _shieldConsumed;

    /// <summary>
    /// Overrides the skin property to return the decorator's skin.
    /// </summary>
    public override ShipSkin Skin => skin;

    /// <summary>
    /// Overrides IsSunk. Camouflage no longer adds extra durability because
    /// the one-time shield provides the additional shot.
    /// </summary>
    public override bool IsSunk => WrappedShip.IsSunk;

    /// <summary>
    /// Camouflage: first hit attempt is absorbed (no damage). Subsequent hits deal damage.
    /// Other skins: delegate to wrapped ship.
    /// </summary>
    public override bool RegisterHit(Position p)
    {
        if (skin != ShipSkin.Camouflage) return WrappedShip.RegisterHit(p);
        if (_shieldConsumed) return WrappedShip.RegisterHit(p);
        _shieldConsumed = true;
        return false;

    }
}

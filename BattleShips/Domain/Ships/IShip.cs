namespace BattleShips.Domain.Ships;

/// <summary>
/// Base interface for all ships in the Decorator pattern.
/// Defines the core contract that all ships (decorated or not) must implement.
/// </summary>
public interface IShip
{
    string Name { get; }
    int Length { get; }
    Position Start { get; }
    Orientation Orientation { get; }
    bool IsSunk { get; }
    ShipKind Kind { get; }
    ShipSkin Skin { get; }

    IEnumerable<Position> Cells();
    void Reposition(Position start, Orientation orientation);
    /// <summary>
    /// Registers a hit attempt at position p.
    /// Returns true if the hit applied damage (counted towards sinking),
    /// false if it was absorbed (e.g., camouflage first-hit shield).
    /// </summary>
    bool RegisterHit(Position p);

    /// <summary>
    /// Gets the number of hits this ship has taken.
    /// Used by decorators to implement enhanced durability.
    /// </summary>
    int HitCount { get; }
}

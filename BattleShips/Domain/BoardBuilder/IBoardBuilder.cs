using BattleShips.Domain.Ships;

namespace BattleShips.Domain.BoardBuilder;

/// <summary>
/// Abstract Builder interface for constructing Board with ships.
/// Defines the interface for building parts of a fleet.
/// </summary>
public interface IBoardBuilder
{
    /// <summary>
    /// Builds a part of the board (places a single ship).
    /// </summary>
    /// <param name="shipKind">The kind of ship to place</param>
    void BuildPart(ShipKind shipKind);

    /// <summary>
    /// Gets the final constructed Board product.
    /// </summary>
    /// <returns>The completed Board with all ships placed</returns>
    Board GetResult();
}


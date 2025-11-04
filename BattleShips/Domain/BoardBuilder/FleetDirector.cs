using BattleShips.Domain.Ships;

namespace BattleShips.Domain.BoardBuilder;

/// <summary>
/// Director class that orchestrates the construction of a fleet on a board.
/// Knows the sequence of construction (what ships to build and in what order).
/// Uses the Builder pattern to construct boards with ships.
/// </summary>
public class FleetDirector
{
    private IBoardBuilder? _builder;

    /// <summary>
    /// Sets the builder to use for construction.
    /// </summary>
    /// <param name="builder">The builder implementation to use</param>
    public void SetBuilder(IBoardBuilder builder)
    {
        _builder = builder;
    }

    /// <summary>
    /// Constructs a complete fleet on the board using the configured builder.
    /// Implements the construction algorithm: iterates through fleet composition
    /// and calls BuildPart() for each ship (as per Builder pattern diagram).
    /// </summary>
    /// <returns>The completed Board with all ships placed</returns>
    public Board Construct()
    {
        if (_builder == null)
            throw new InvalidOperationException("Builder must be set before constructing. Call SetBuilder() first.");

        // Construction algorithm: for all ships in fleet composition, build each part
        foreach (var shipKind in DefaultFleet.Composition)
        {
            _builder.BuildPart(shipKind);
        }

        return _builder.GetResult();
    }

    /// <summary>
    /// Constructs a fleet using a custom composition.
    /// </summary>
    /// <param name="composition">Custom fleet composition (sequence of ship kinds)</param>
    /// <returns>The completed Board with all ships placed</returns>
    public Board Construct(IEnumerable<ShipKind> composition)
    {
        if (_builder == null)
            throw new InvalidOperationException("Builder must be set before constructing. Call SetBuilder() first.");

        // Construction algorithm: for all ships in provided composition, build each part
        foreach (var shipKind in composition)
        {
            _builder.BuildPart(shipKind);
        }

        return _builder.GetResult();
    }
}


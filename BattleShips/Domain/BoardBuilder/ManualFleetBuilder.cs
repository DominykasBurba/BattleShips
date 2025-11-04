using BattleShips.Domain.Ships;

namespace BattleShips.Domain.BoardBuilder;

/// <summary>
/// Concrete Builder for manually placing ships on a board.
/// Ships are placed with specific positions and orientations.
/// </summary>
public class ManualFleetBuilder : IBoardBuilder
{
    private readonly Board _board;
    private Position? _pendingPosition;
    private Orientation? _pendingOrientation;

    /// <summary>
    /// Creates a new ManualFleetBuilder that will work with an existing board.
    /// </summary>
    /// <param name="board">The board to place ships on</param>
    public ManualFleetBuilder(Board board)
    {
        _board = board;
        _board.Clear(); // Clear existing ships before building
    }

    /// <summary>
    /// Sets the position and orientation for the next ship to be placed.
    /// Used when calling BuildPart() without parameters.
    /// </summary>
    public void SetNextShipPlacement(Position position, Orientation orientation)
    {
        _pendingPosition = position;
        _pendingOrientation = orientation;
    }

    public void BuildPart(ShipKind shipKind)
    {
        if (_pendingPosition == null || _pendingOrientation == null)
            throw new InvalidOperationException("Position and orientation must be set before placing a ship. Use SetNextShipPlacement() or TryBuildPart() instead.");

        BuildPart(shipKind, _pendingPosition.Value, _pendingOrientation.Value);
        
        // Clear pending values after successful placement
        _pendingPosition = null;
        _pendingOrientation = null;
    }

    /// <summary>
    /// Builds a part (places a ship) with explicit position and orientation.
    /// </summary>
    public void BuildPart(ShipKind shipKind, Position position, Orientation orientation)
    {
        var ship = ShipFactory.Create(shipKind, position, orientation);
        
        if (!_board.Place(ship))
            throw new InvalidOperationException($"Failed to place {shipKind} at position {position} with orientation {orientation}.");
    }

    /// <summary>
    /// Attempts to place a ship at the specified position and orientation.
    /// Returns true if successful, false otherwise (doesn't throw).
    /// </summary>
    public bool TryBuildPart(ShipKind shipKind, Position position, Orientation orientation)
    {
        var ship = ShipFactory.Create(shipKind, position, orientation);
        return _board.Place(ship);
    }

    public Board GetResult()
    {
        return _board;
    }
}


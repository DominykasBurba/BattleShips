using BattleShips.Domain.Ships;
using BattleShips.Domain.Ships.Factories;

namespace BattleShips.Domain.BoardBuilder;

/// <summary>
/// Concrete Builder for manually placing ships on a board.
/// Ships are placed with specific positions and orientations.
/// </summary>
public class ManualFleetBuilder : IBoardBuilder
{
    private readonly Board _board;
    private readonly IShipFactory _shipFactory;
    private Position? _pendingPosition;
    private Orientation? _pendingOrientation;

    /// <summary>
    /// Creates a new ManualFleetBuilder that will work with an existing board.
    /// </summary>
    /// <param name="board">The board to place ships on</param>
    /// <param name="shipFactory">The factory to use for creating ships (defaults to ClassicShipFactory if not provided)</param>
    public ManualFleetBuilder(Board board, IShipFactory? shipFactory = null)
    {
        _board = board;
        _board.Clear(); // Clear existing ships before building
        _shipFactory = shipFactory ?? new ClassicShipFactory();
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
        var ship = _shipFactory.CreateShip(shipKind, position, orientation);

        if (!_board.Place(ship))
            throw new InvalidOperationException($"Failed to place {shipKind} at position {position} with orientation {orientation}.");
    }

    /// <summary>
    /// Attempts to place a ship at the specified position and orientation.
    /// Returns true if successful, false otherwise (doesn't throw).
    /// </summary>
    public bool TryBuildPart(ShipKind shipKind, Position position, Orientation orientation)
    {
        var ship = _shipFactory.CreateShip(shipKind, position, orientation);
        return _board.Place(ship);
    }

    public Board GetResult()
    {
        return _board;
    }
}


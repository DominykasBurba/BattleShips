using BattleShips.Domain.Ships;
using BattleShips.Domain.Ships.Factories;

namespace BattleShips.Domain.BoardBuilder;

public class ManualFleetBuilder : IBoardBuilder
{
    private readonly Board _board;
    private readonly IShipFactory _shipFactory;
    private Position? _pendingPosition;
    private Orientation? _pendingOrientation;
    
    public ManualFleetBuilder(Board board, IShipFactory? shipFactory = null)
    {
        _board = board;
        _board.Clear();
        _shipFactory = shipFactory ?? new ClassicShipFactory();
    }
    

    public void BuildPart(ShipKind shipKind)
    {
        if (_pendingPosition == null || _pendingOrientation == null)
            throw new InvalidOperationException("Position and orientation must be set before placing a ship. Use SetNextShipPlacement() or TryBuildPart() instead.");

        BuildPart(shipKind, _pendingPosition.Value, _pendingOrientation.Value);
        
        _pendingPosition = null;
        _pendingOrientation = null;
    }
    
    public void BuildPart(ShipKind shipKind, Position position, Orientation orientation)
    {
        var ship = _shipFactory.CreateShip(shipKind, position, orientation);

        if (!_board.Place(ship))
            throw new InvalidOperationException($"Failed to place {shipKind} at position {position} with orientation {orientation}.");
    }

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


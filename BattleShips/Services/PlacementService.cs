using BattleShips.Domain;
using BattleShips.Domain.BoardBuilder;
using BattleShips.Domain.Ships;

namespace BattleShips.Services;

public class PlacementService
{
    private readonly FleetDirector _director = new();

    /// <summary>
    /// Randomizes a fleet on the given board using the Builder pattern.
    /// Uses FleetDirector with RandomFleetBuilder to construct the fleet.
    /// </summary>
    public void RandomizeFleet(Board board)
    {
        // Use Builder pattern: Director orchestrates construction with Random builder
        var builder = new RandomFleetBuilder(board);
        _director.SetBuilder(builder);
        _director.Construct(); // Director calls BuildPart() for each ship in fleet composition
    }

    /// <summary>
    /// Attempts to place a ship on the board.
    /// </summary>
    public bool TryPlace(Board board, ShipBase ship) => board.Place(ship);

    /// <summary>
    /// Attempts to rotate a ship on the board.
    /// </summary>
    public bool TryRotate(Board board, ShipBase ship)
    {
        board.Remove(ship);
        var newO = ship.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
        ship.Reposition(ship.Start, newO);
        if (board.Place(ship)) return true;

        // rollback if invalid
        ship.Reposition(ship.Start, newO == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
        board.Place(ship);
        return false;
    }
}
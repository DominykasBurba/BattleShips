using BattleShips.Domain;
using BattleShips.Domain.BoardBuilder;
using BattleShips.Domain.Ships;
using BattleShips.Domain.Ships.Factories;

namespace BattleShips.Services;

public class PlacementService
{
    private readonly FleetDirector _director = new();
    private ShipType _shipType = ShipType.Classic;
    private ShipSkin _shipSkin = ShipSkin.Default;

    /// <summary>
    /// Sets the ship type to use for future fleet randomizations.
    /// </summary>
    public void SetShipType(ShipType shipType)
    {
        _shipType = shipType;
    }

    /// <summary>
    /// Sets the ship skin to use for future fleet randomizations.
    /// </summary>
    public void SetShipSkin(ShipSkin shipSkin)
    {
        _shipSkin = shipSkin;
    }

    /// <summary>
    /// Randomizes a fleet on the given board using the Builder pattern.
    /// Uses FleetDirector with RandomFleetBuilder to construct the fleet.
    /// Uses the configured ship type (Classic or Modern) to create ships.
    /// </summary>
    public void RandomizeFleet(Board board)
    {
        // Create the appropriate factory based on ship type
        IShipFactory factory = _shipType switch
        {
            ShipType.Modern => new ModernShipFactory(),
            _ => new ClassicShipFactory()
        };

        // Use Builder pattern: Director orchestrates construction with Random builder
        var builder = new RandomFleetBuilder(board, factory, _shipSkin);
        _director.SetBuilder(builder);
        _director.Construct(); // Director calls BuildPart() for each ship in fleet composition
    }

    /// <summary>
    /// Attempts to place a ship on the board.
    /// </summary>
    public bool TryPlace(Board board, IShip ship) => board.Place(ship);

    /// <summary>
    /// Attempts to rotate a ship on the board.
    /// </summary>
    public bool TryRotate(Board board, IShip ship)
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
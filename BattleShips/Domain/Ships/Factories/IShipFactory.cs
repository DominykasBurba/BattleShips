namespace BattleShips.Domain.Ships.Factories;

/// <summary>
/// Abstract Factory interface for creating different types of ships.
/// Encapsulates ship creation logic following the Abstract Factory pattern.
/// </summary>
public interface IShipFactory
{
    ShipBase CreateBattleship(Position start, Orientation orientation);
    ShipBase CreateSubmarine(Position start, Orientation orientation);
    ShipBase CreateDestroyer(Position start, Orientation orientation);
    ShipBase CreateCruiser(Position start, Orientation orientation);

    /// <summary>
    /// Creates a ship based on the ShipKind enum.
    /// </summary>
    ShipBase CreateShip(ShipKind kind, Position start, Orientation orientation);
}

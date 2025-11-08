namespace BattleShips.Domain.Ships.Factories;

/// <summary>
/// Abstract Factory interface for creating different types of ships.
/// Encapsulates ship creation logic following the Abstract Factory pattern.
/// Returns IShip to support decorator pattern.
/// </summary>
public interface IShipFactory
{
    IShip CreateBattleship(Position start, Orientation orientation);
    IShip CreateSubmarine(Position start, Orientation orientation);
    IShip CreateDestroyer(Position start, Orientation orientation);
    IShip CreateCruiser(Position start, Orientation orientation);

    /// <summary>
    /// Creates a ship based on the ShipKind enum.
    /// </summary>
    IShip CreateShip(ShipKind kind, Position start, Orientation orientation);
}

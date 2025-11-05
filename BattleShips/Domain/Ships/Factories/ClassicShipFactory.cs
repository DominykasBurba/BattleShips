namespace BattleShips.Domain.Ships.Factories;

/// <summary>
/// Concrete Factory for creating classic battleship game ships.
/// Part of the Abstract Factory pattern - creates the "Classic" family of ships.
/// This is the traditional Battleship game implementation.
/// </summary>
public class ClassicShipFactory : IShipFactory
{
    public ShipBase CreateBattleship(Position start, Orientation orientation)
        => new Battleship(start, orientation);

    public ShipBase CreateSubmarine(Position start, Orientation orientation)
        => new Submarine(start, orientation);

    public ShipBase CreateDestroyer(Position start, Orientation orientation)
        => new Destroyer(start, orientation);

    public ShipBase CreateCruiser(Position start, Orientation orientation)
        => new Cruiser(start, orientation);

    public ShipBase CreateShip(ShipKind kind, Position start, Orientation orientation) => kind switch
    {
        ShipKind.Battleship => CreateBattleship(start, orientation),
        ShipKind.Submarine => CreateSubmarine(start, orientation),
        ShipKind.Destroyer => CreateDestroyer(start, orientation),
        ShipKind.Cruiser => CreateCruiser(start, orientation),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown ship kind")
    };
}

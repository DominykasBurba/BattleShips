namespace BattleShips.Domain.Ships.Factories;

using BattleShips.Domain.Ships.Modern;

/// <summary>
/// Concrete Factory for creating modern naval ships with enhanced capabilities.
/// Part of the Abstract Factory pattern - creates the "Modern" family of ships.
/// Modern ships are larger and more powerful than their classic counterparts.
/// </summary>
public class ModernShipFactory : IShipFactory
{
    public IShip CreateBattleship(Position start, Orientation orientation)
        => new ModernBattleship(start, orientation);

    public IShip CreateSubmarine(Position start, Orientation orientation)
        => new ModernSubmarine(start, orientation);

    public IShip CreateDestroyer(Position start, Orientation orientation)
        => new ModernDestroyer(start, orientation);

    public IShip CreateCruiser(Position start, Orientation orientation)
        => new ModernCruiser(start, orientation);

    public IShip CreateShip(ShipKind kind, Position start, Orientation orientation) => kind switch
    {
        ShipKind.Battleship => CreateBattleship(start, orientation),
        ShipKind.Submarine => CreateSubmarine(start, orientation),
        ShipKind.Destroyer => CreateDestroyer(start, orientation),
        ShipKind.Cruiser => CreateCruiser(start, orientation),
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unknown ship kind")
    };
}

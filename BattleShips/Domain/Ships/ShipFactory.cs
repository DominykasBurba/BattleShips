namespace BattleShips.Domain.Ships;

public static class ShipFactory
{
    public static ShipBase Create(ShipKind kind, BattleShips.Domain.Position start, BattleShips.Domain.Orientation o) => kind switch
    {
        ShipKind.Battleship => new Battleship(start, o),
        ShipKind.Submarine  => new Submarine(start, o),
        ShipKind.Destroyer  => new Destroyer(start, o),
        ShipKind.Cruiser    => new Cruiser(start, o),
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };
}
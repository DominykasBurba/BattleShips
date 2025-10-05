namespace BattleShips.Domain.Ships;

public enum ShipKind { Carrier, Battleship, Cruiser, Submarine, Destroyer }

public static class DefaultFleet
{
    public static readonly ShipKind[] Composition =
    [
        ShipKind.Battleship,                      // 1 × length 4
        ShipKind.Submarine, ShipKind.Submarine,   // 2 × length 3
        ShipKind.Destroyer, ShipKind.Destroyer, ShipKind.Destroyer, // 3 × length 2
        ShipKind.Cruiser, ShipKind.Cruiser, ShipKind.Cruiser, ShipKind.Cruiser // 4 × length 1
    ];
}
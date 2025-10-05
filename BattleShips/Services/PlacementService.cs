using BattleShips.Domain;
using BattleShips.Domain.Ships;

namespace BattleShips.Services;

public class PlacementService
{
    private readonly Random _rng = new();

    public void RandomizeFleet(Board board)
    {
        board.Clear();
        foreach (var kind in DefaultFleet.Composition)
        {
            bool placed = false;
            for (int tries = 0; tries < 300 && !placed; tries++)
            {
                var o = _rng.Next(2) == 0 ? Orientation.Horizontal : Orientation.Vertical;
                var length = ShipFactory.Create(kind, new Position(0,0), o).Length;
                var maxR = o == Orientation.Horizontal ? board.Size : board.Size - length + 1;
                var maxC = o == Orientation.Horizontal ? board.Size - length + 1 : board.Size;
                var start = new Position(_rng.Next(maxR), _rng.Next(maxC));

                var ship = ShipFactory.Create(kind, start, o);
                placed = board.Place(ship);
            }
            if (!placed) throw new InvalidOperationException("Failed to place fleet randomly.");
        }
    }

    public bool TryPlace(Board board, ShipBase ship) => board.Place(ship);

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
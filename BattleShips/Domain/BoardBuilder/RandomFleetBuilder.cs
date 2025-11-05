using BattleShips.Domain.Ships;
using BattleShips.Domain.Ships.Factories;

namespace BattleShips.Domain.BoardBuilder;

/// <summary>
/// Concrete Builder for randomly placing ships on a board.
/// Implements the random placement strategy.
/// </summary>
public class RandomFleetBuilder : IBoardBuilder
{
    private readonly Board _board;
    private readonly Random _rng;
    private readonly IShipFactory _shipFactory;

    /// <summary>
    /// Creates a new RandomFleetBuilder that will work with an existing board.
    /// </summary>
    /// <param name="board">The board to place ships on</param>
    /// <param name="shipFactory">The factory to use for creating ships (defaults to ClassicShipFactory if not provided)</param>
    public RandomFleetBuilder(Board board, IShipFactory? shipFactory = null)
    {
        _board = board;
        _board.Clear(); // Clear existing ships before building
        _rng = new Random();
        _shipFactory = shipFactory ?? new ClassicShipFactory();
    }

    public void BuildPart(ShipKind shipKind)
    {
        bool placed = false;
        for (int tries = 0; tries < 300 && !placed; tries++)
        {
            // Randomly choose orientation
            var orientation = _rng.Next(2) == 0 ? Orientation.Horizontal : Orientation.Vertical;

            // Calculate valid position range
            var length = _shipFactory.CreateShip(shipKind, new Position(0, 0), orientation).Length;
            var maxR = orientation == Orientation.Horizontal ? _board.Size : _board.Size - length + 1;
            var maxC = orientation == Orientation.Horizontal ? _board.Size - length + 1 : _board.Size;
            var start = new Position(_rng.Next(maxR), _rng.Next(maxC));

            // Create and place ship
            var ship = _shipFactory.CreateShip(shipKind, start, orientation);
            placed = _board.Place(ship);
        }

        if (!placed)
            throw new InvalidOperationException($"Failed to place {shipKind} randomly after 300 attempts.");
    }

    public Board GetResult()
    {
        return _board;
    }
}


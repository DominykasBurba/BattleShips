using BattleShips.Domain.Ships;
using BattleShips.Domain.Ships.Factories;
using BattleShips.Domain.Ships.Decorators;

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
    private readonly ShipSkin _shipSkin;

    /// <summary>
    /// Creates a new RandomFleetBuilder that will work with an existing board.
    /// </summary>
    /// <param name="board">The board to place ships on</param>
    /// <param name="shipFactory">The factory to use for creating ships (defaults to ClassicShipFactory if not provided)</param>
    /// <param name="shipSkin">The skin to apply to ships using Decorator pattern (defaults to Default)</param>
    public RandomFleetBuilder(Board board, IShipFactory? shipFactory = null, ShipSkin shipSkin = ShipSkin.Default)
    {
        _board = board;
        _board.Clear(); // Clear existing ships before building
        _rng = new Random();
        _shipFactory = shipFactory ?? new ClassicShipFactory();
        _shipSkin = shipSkin;
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

            // Create ship
            IShip ship = _shipFactory.CreateShip(shipKind, start, orientation);

            // Apply Decorator pattern: wrap ship with skin decorator if not default
            if (_shipSkin != ShipSkin.Default)
            {
                ship = new SkinnedShipDecorator(ship, _shipSkin);
            }

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


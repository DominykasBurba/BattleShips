using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Iterator;

/// <summary>
/// Concrete aggregate in the Iterator pattern.
/// Represents a collection of ships that can be iterated.
/// </summary>
public class ShipCollection : IAggregate<IShip>
{
    private readonly IReadOnlyList<IShip> _ships;

    public ShipCollection(IReadOnlyList<IShip> ships)
    {
        _ships = ships;
    }

    /// <summary>
    /// Creates an iterator for traversing all ships in the collection.
    /// </summary>
    /// <returns>An iterator for the ships</returns>
    public IIterator<IShip> CreateIterator()
    {
        return new ShipListIterator(_ships);
    }
}


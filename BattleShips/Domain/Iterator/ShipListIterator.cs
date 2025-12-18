using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Iterator;

/// <summary>
/// Concrete iterator for iterating through a List of ships (List&lt;IShip&gt;).
/// </summary>
public sealed class ShipListIterator : IIterator<IShip>
{
    private readonly IReadOnlyList<IShip> _ships;
    private int _currentIndex;

    public ShipListIterator(IReadOnlyList<IShip> ships)
    {
        _ships = ships;
        _currentIndex = 0;
    }

    public void First()
    {
        _currentIndex = 0;
    }

    public void Next()
    {
        _currentIndex++;
    }

    public bool IsDone()
    {
        return _currentIndex >= _ships.Count;
    }

    public IShip CurrentItem()
    {
        if (IsDone())
            throw new InvalidOperationException("Iterator is done. No current item.");

        return _ships[_currentIndex];
    }
}


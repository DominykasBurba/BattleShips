namespace BattleShips.Domain.Iterator;

/// <summary>
/// Concrete iterator for iterating through a HashSet of positions (HashSet&lt;Position&gt;).
/// </summary>
public sealed class PositionSetIterator : IIterator<Position>
{
    private readonly HashSet<Position> _positions;
    private Position[] _positionArray;
    private int _currentIndex;

    public PositionSetIterator(HashSet<Position> positions)
    {
        _positions = positions;
        _positionArray = positions.ToArray();
        _currentIndex = 0;
    }

    public void First()
    {
        _currentIndex = 0;
        // Refresh array in case set was modified
        _positionArray = _positions.ToArray();
    }

    public void Next()
    {
        _currentIndex++;
    }

    public bool IsDone()
    {
        return _currentIndex >= _positionArray.Length;
    }

    public Position CurrentItem()
    {
        if (IsDone())
            throw new InvalidOperationException("Iterator is done. No current item.");

        return _positionArray[_currentIndex];
    }
}


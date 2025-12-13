namespace BattleShips.Domain.Iterator;

/// <summary>
/// Concrete aggregate in the Iterator pattern.
/// Represents a set of positions that can be iterated.
/// </summary>
public class PositionSet : IAggregate<Position>
{
    private readonly HashSet<Position> _positions;

    public PositionSet(HashSet<Position> positions)
    {
        _positions = positions;
    }

    /// <summary>
    /// Creates an iterator for traversing all positions in the set.
    /// </summary>
    /// <returns>An iterator for the positions</returns>
    public IIterator<Position> CreateIterator()
    {
        return new PositionSetIterator(_positions);
    }
}


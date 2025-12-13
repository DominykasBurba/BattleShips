namespace BattleShips.Domain.Iterator;

/// <summary>
/// Aggregate interface in the Iterator pattern.
/// Defines the interface for creating an iterator.
/// </summary>
/// <typeparam name="T">The type of elements in the aggregate</typeparam>
public interface IAggregate<out T>
{
    /// <summary>
    /// Creates an iterator for this aggregate.
    /// </summary>
    /// <returns>An iterator for traversing the aggregate's elements</returns>
    IIterator<T> CreateIterator();
}


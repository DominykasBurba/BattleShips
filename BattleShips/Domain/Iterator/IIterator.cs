namespace BattleShips.Domain.Iterator;

/// <summary>
/// Iterator interface in the Iterator pattern.
/// Defines the interface for accessing and traversing elements.
/// </summary>
/// <typeparam name="T">The type of elements being iterated</typeparam>
public interface IIterator<out T>
{
    /// <summary>
    /// Resets the iterator to the first element.
    /// </summary>
    void First();

    /// <summary>
    /// Advances the iterator to the next element.
    /// </summary>
    void Next();

    /// <summary>
    /// Checks if the traversal is complete (no more elements).
    /// </summary>
    /// <returns>True if there are no more elements, false otherwise</returns>
    bool IsDone();

    /// <summary>
    /// Returns the current element.
    /// </summary>
    /// <returns>The current element</returns>
    T CurrentItem();
}


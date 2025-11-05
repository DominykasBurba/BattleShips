namespace BattleShips.Domain.Observer;

/// <summary>
/// Abstract Observer interface.
/// Defines the interface for objects that should be notified of changes in a Subject.
/// </summary>
public interface IObserver
{
    /// <summary>
    /// Called by the Subject to notify the observer of a state change.
    /// </summary>
    void Update();
}


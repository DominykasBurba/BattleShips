namespace BattleShips.Domain.Observer;

/// <summary>
/// Abstract Subject class that maintains a list of observers and notifies them of changes.
/// Implements the Subject part of the Observer pattern.
/// </summary>
public abstract class Subject
{
    private readonly List<IObserver> _observers = new();

    /// <summary>
    /// Attaches an observer to the subject.
    /// </summary>
    /// <param name="observer">The observer to attach</param>
    public void Attach(IObserver observer)
    {
        if (observer != null && !_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    /// <summary>
    /// Detaches an observer from the subject.
    /// </summary>
    /// <param name="observer">The observer to detach</param>
    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
    }

    /// <summary>
    /// Notifies all attached observers of a state change.
    /// Implements the algorithm: for all observers, call update().
    /// </summary>
    protected void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.Update();
        }
    }
}


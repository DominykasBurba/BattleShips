namespace BattleShips.Domain.Observer;


public abstract class Subject
{
    private readonly List<IObserver> _observers = new();
    
    public void Attach(IObserver observer)
    {
        if (observer != null && !_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }
    
    public void Detach(IObserver observer)
    {
        _observers.Remove(observer);
    }

    protected void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.Update();
        }
    }
}


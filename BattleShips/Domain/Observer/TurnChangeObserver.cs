using BattleShips.Domain;

namespace BattleShips.Domain.Observer;

/// <summary>
/// ConcreteObserver that observes turn changes in GameSession.
/// Tracks the current player and reacts to turn changes.
/// </summary>
public class TurnChangeObserver : IObserver
{
    private readonly GameSession _subject;
    private Player? _observerState;

    public TurnChangeObserver(GameSession subject)
    {
        _subject = subject;
        _observerState = subject.Current;
        _subject.Attach(this);
    }

    public void Update()
    {
        // Observer retrieves state from subject (as per diagram: observerState = subject->getState())
        _observerState = _subject.Current;
        
        // React to turn change
        Console.WriteLine($"[TurnChangeObserver] Turn changed to: {_observerState?.Name ?? "Unknown"}");
    }

    public Player? GetState() => _observerState;
}


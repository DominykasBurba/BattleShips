using BattleShips.Domain;

namespace BattleShips.Domain.Observer;

/// <summary>
/// ConcreteObserver that observes GameSession state changes.
/// Logs and tracks game state changes.
/// </summary>
public class GameStateObserver : IObserver
{
    private readonly GameSession _subject;
    private Phase _observerState;

    public GameStateObserver(GameSession subject)
    {
        _subject = subject;
        _observerState = subject.Phase;
        _subject.Attach(this);
    }

    public void Update()
    {
        // Observer retrieves state from subject (as per diagram: observerState = subject->getState())
        _observerState = _subject.Phase;
        
        // React to state change - could log, update UI, etc.
        Console.WriteLine($"[GameStateObserver] Game state changed to: {_observerState}");
    }

    public Phase GetState() => _observerState;
}


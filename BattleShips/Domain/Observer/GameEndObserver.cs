using BattleShips.Domain;

namespace BattleShips.Domain.Observer;

/// <summary>
/// ConcreteObserver that observes game end events.
/// Reacts when a winner is determined or game finishes.
/// </summary>
public class GameEndObserver : IObserver
{
    private readonly GameSession _subject;
    private Player? _observerState;

    public GameEndObserver(GameSession subject)
    {
        _subject = subject;
        _observerState = _subject.Winner;
        _subject.Attach(this);
    }

    public void Update()
    {
        // Observer retrieves state from subject (as per diagram: observerState = subject->getState())
        _observerState = _subject.Winner;
        
        // React to game end
        if (_subject.Phase == Phase.Finished)
        {
            if (_observerState != null)
            {
                Console.WriteLine($"[GameEndObserver] Game finished! Winner: {_observerState.Name}");
            }
            else
            {
                Console.WriteLine("[GameEndObserver] Game finished! Draw accepted.");
            }
        }
    }

    public Player? GetState() => _observerState;
}


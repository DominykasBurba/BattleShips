using BattleShips.Domain;

namespace BattleShips.Domain.Observer;


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
        // gaunam state is subjecto
        _observerState = _subject.Phase;
        
        Console.WriteLine($"[GameStateObserver] Game state changed to: {_observerState}");
    }

    public Phase GetState() => _observerState;
}


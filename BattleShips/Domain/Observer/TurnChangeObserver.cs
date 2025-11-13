using BattleShips.Domain;

namespace BattleShips.Domain.Observer;


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
        _observerState = _subject.Current;
        
        Console.WriteLine($"[TurnChangeObserver] Turn changed to: {_observerState?.Name ?? "Unknown"}");
    }

    public Player? GetState() => _observerState;
}


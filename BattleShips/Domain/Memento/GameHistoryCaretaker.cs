namespace BattleShips.Domain.Memento;

/// <summary>
/// Caretaker in the Memento pattern. Keeps a history of game session snapshots.
/// </summary>
public sealed class GameHistoryCaretaker
{
    private readonly Stack<GameSessionMemento> _history = new();

    public void Save(GameSession session)
    {
        _history.Push(session.CreateMemento());
    }

    public bool CanUndo => _history.Count > 0;

    public void Undo(GameSession session)
    {
        if (_history.Count == 0) return;
        var memento = _history.Pop();
        session.Restore(memento);
    }
}




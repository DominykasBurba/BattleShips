using BattleShips.Domain;
using BattleShips.Domain.Memento;

namespace BattleShips.Services;

/// <summary>
/// Concrete Mediator that coordinates GameService, ChatService and Memento caretaker.
/// UI (Blazor komponentai, hub'ai) turėtų kalbėtis tik su šia klase, o ne su atskirais servisais.
/// </summary>
public sealed class GameMediator : IGameMediator
{
    private readonly GameService _gameService;
    private readonly ChatService _chatService;
    private readonly GameHistoryCaretaker _history = new();

    public GameMediator(GameService gameService, ChatService chatService)
    {
        _gameService = gameService;
        _chatService = chatService;
    }

    public GameSession? Session => _gameService.Session;

    public IReadOnlyList<ChatMessage> ChatMessages => _chatService.Messages;

    public void StartLocalGame(
        int size = 10,
        bool enemyIsAi = true,
        ShipType shipType = ShipType.Classic,
        ShipSkin shipSkin = ShipSkin.Default)
    {
        _gameService.NewLocalSession(size, enemyIsAi, shipType, shipSkin);
        _history.SaveIfPossible(Session);
    }

    public void RandomizeForCurrentPlayer()
    {
        if (Session is null) return;
        _gameService.RandomizeFor(Session.Current);
        _history.SaveIfPossible(Session);
    }

    public ShotResult FireAt(Position pos)
    {
        if (Session is null) return ShotResult.Invalid;

        _history.SaveIfPossible(Session);
        return _gameService.FireAt(pos);
    }

    public void Surrender()
    {
        if (Session is null) return;
        _history.SaveIfPossible(Session);
        _gameService.Surrender(Session.Current);
    }

    public void ProposeDraw()
    {
        if (Session is null) return;
        _history.SaveIfPossible(Session);
        _gameService.ProposeDraw(Session.Current);
    }

    public void AcceptDraw()
    {
        if (Session is null) return;
        _history.SaveIfPossible(Session);
        _gameService.AcceptDraw(Session.Current);
    }

    public void SendChatMessage(string sender, string text)
    {
        var message = new ChatMessage(sender, text, DateTime.UtcNow);
        _chatService.SendMessage(message);
    }

    public bool CanUndo => Session is not null && _history.CanUndo;

    public void UndoLastMove()
    {
        if (Session is null) return;
        _history.Undo(Session);
    }
}

internal static class GameMediatorExtensions
{
    /// <summary>
    /// Helper extension so that Mediator nesprogtų, jei sesija dar nesukurta.
    /// </summary>
    public static void SaveIfPossible(this GameHistoryCaretaker caretaker, GameSession? session)
    {
        if (session is null) return;
        caretaker.Save(session);
    }
}



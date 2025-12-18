using BattleShips.Domain;
using BattleShips.Domain.Memento;

namespace BattleShips.Services;

/// <summary>
/// Mediator interface that coordinates interactions between game-related services
/// (GameService, ChatService, PlacementService, Memento caretaker, etc.).
/// UI sluoksnis bendrauja tik per šį interfeisą.
/// </summary>
public interface IGameMediator
{
    GameSession? Session { get; }

    IReadOnlyList<ChatMessage> ChatMessages { get; }

    void StartLocalGame(
        int size = 10,
        bool enemyIsAi = true,
        ShipType shipType = ShipType.Classic,
        ShipSkin shipSkin = ShipSkin.Default);

    void RandomizeForCurrentPlayer();

    ShotResult FireAt(Position pos);

    void Surrender();

    void ProposeDraw();

    void AcceptDraw();

    void SendChatMessage(string sender, string text);

    bool CanUndo { get; }

    void UndoLastMove();
}




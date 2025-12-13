using BattleShips.Domain.Observer;

namespace BattleShips.Domain.GameInitialization;

/// <summary>
/// Concrete class implementing online game initialization algorithm.
/// Sealed class as required by Template Method pattern specification.
/// </summary>
public sealed class OnlineGameInitializer : AbstractGameInitializer
{
    protected override (Player p1, Player p2) CreatePlayers(int boardSize)
    {
        // For online games, both players are human
        var p1 = new HumanPlayer("Player 1", boardSize);
        var p2 = new HumanPlayer("Player 2", boardSize);
        
        return (p1, p2);
    }

    protected override GameSession CreateSession(Player p1, Player p2)
    {
        // Use Singleton pattern to get/create session
        return GameSession.GetInstance(p1, p2);
    }

    protected override void ConfigureSettings(GameSession session, ShipType shipType, ShootingMode shootingMode)
    {
        // For online games, settings are configured externally via GameLobbyService
        // This step is a placeholder - actual configuration happens in GameLobbyService
        // after initialization
    }

    protected override void AttachObservers(GameSession session)
    {
        // Attach observers for online games
        _ = new GameStateObserver(session);
        _ = new TurnChangeObserver(session);
        _ = new GameEndObserver(session);
    }

    protected override void InitializeGameState(GameSession session)
    {
        // Online games start in Preparation phase (default)
        // No additional initialization needed
    }
}


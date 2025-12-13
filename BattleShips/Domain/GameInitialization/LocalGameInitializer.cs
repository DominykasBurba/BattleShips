using BattleShips.Domain.Observer;

namespace BattleShips.Domain.GameInitialization;

/// <summary>
/// Concrete class implementing local game initialization algorithm.
/// Sealed class as required by Template Method pattern specification.
/// </summary>
public sealed class LocalGameInitializer : AbstractGameInitializer
{
    private readonly bool _enemyIsAi;
    private readonly ShipSkin _shipSkin;

    public LocalGameInitializer(bool enemyIsAi = true, ShipSkin shipSkin = ShipSkin.Default)
    {
        _enemyIsAi = enemyIsAi;
        _shipSkin = shipSkin;
    }

    protected override (Player p1, Player p2) CreatePlayers(int boardSize)
    {
        var p1 = new HumanPlayer("Player 1", boardSize);
        Player p2 = _enemyIsAi 
            ? new AiPlayer("Enemy AI", boardSize) 
            : new HumanPlayer("Player 2", boardSize);
        
        return (p1, p2);
    }

    protected override GameSession CreateSession(Player p1, Player p2)
    {
        // Use Singleton pattern to get/create session
        return GameSession.GetInstance(p1, p2);
    }

    protected override void ConfigureSettings(GameSession session, ShipType shipType, ShootingMode shootingMode)
    {
        // For local games, settings are configured externally via GameService
        // This step is a placeholder - actual configuration happens in GameService
        // after initialization
    }

    protected override void AttachObservers(GameSession session)
    {
        // Attach observers for local games
        _ = new GameStateObserver(session);
        _ = new TurnChangeObserver(session);
        _ = new GameEndObserver(session);
    }

    protected override void InitializeGameState(GameSession session)
    {
        // Local games start in Preparation phase (default)
        // No additional initialization needed
    }
}


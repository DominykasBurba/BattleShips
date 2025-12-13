using BattleShips.Domain.Observer;

namespace BattleShips.Domain.GameInitialization;

/// <summary>
/// Abstract class in the Template Method pattern.
/// Defines the skeleton of the game initialization algorithm in the template method.
/// </summary>
public abstract class AbstractGameInitializer
{
    /// <summary>
    /// Template method that defines the algorithm structure.
    /// This method cannot be overridden (not marked as virtual or abstract).
    /// </summary>
    /// <param name="boardSize">Size of the game board</param>
    /// <param name="shipType">Type of ships to use</param>
    /// <param name="shootingMode">Shooting mode for the game</param>
    /// <returns>Initialized GameSession</returns>
    public GameSession InitializeGame(int boardSize, ShipType shipType, ShootingMode shootingMode = ShootingMode.Single)
    {
        // Step 1: Create players (primitive operation - varies by game type)
        var (p1, p2) = CreatePlayers(boardSize);

        // Step 2: Create session (primitive operation)
        var session = CreateSession(p1, p2);

        // Step 3: Attach observers (hook operation - optional, has default)
        AttachObservers(session);

        // Step 4: Configure game settings (primitive operation - varies by game type)
        ConfigureSettings(session, shipType, shootingMode);

        // Step 5: Initialize game state (hook operation - optional, has default)
        InitializeGameState(session);

        return session;
    }

    /// <summary>
    /// Primitive operation: Creates the players for the game.
    /// Must be implemented by concrete classes.
    /// </summary>
    protected abstract (Player p1, Player p2) CreatePlayers(int boardSize);

    /// <summary>
    /// Primitive operation: Creates the game session.
    /// Must be implemented by concrete classes.
    /// </summary>
    protected abstract GameSession CreateSession(Player p1, Player p2);

    /// <summary>
    /// Primitive operation: Configures game settings.
    /// Must be implemented by concrete classes.
    /// </summary>
    protected abstract void ConfigureSettings(GameSession session, ShipType shipType, ShootingMode shootingMode);

    /// <summary>
    /// Hook operation: Attaches observers to the session (optional, default does nothing).
    /// Can be overridden by concrete classes.
    /// </summary>
    protected virtual void AttachObservers(GameSession session)
    {
        // Default: do nothing
        // Subclasses can override to attach observers
    }

    /// <summary>
    /// Hook operation: Initializes additional game state (optional, default does nothing).
    /// Can be overridden by concrete classes.
    /// </summary>
    protected virtual void InitializeGameState(GameSession session)
    {
        // Default: do nothing
        // Subclasses can override to initialize additional state
    }
}


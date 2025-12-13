namespace BattleShips.Domain.State;

/// <summary>
/// State interface in the State pattern.
/// Defines the common interface for all concrete game states.
/// </summary>
public abstract class IGameState
{
    /// <summary>
    /// Gets the phase enum value that this state represents.
    /// </summary>
    public abstract Phase Phase { get; }

    /// <summary>
    /// Handles the request to start the game.
    /// </summary>
    /// <param name="context">The game session context</param>
    /// <returns>True if the game can start, false otherwise</returns>
    public abstract bool HandleStart(GameSession context);

    /// <summary>
    /// Handles firing at a position.
    /// </summary>
    /// <param name="context">The game session context</param>
    /// <param name="pos">The position to fire at</param>
    /// <returns>The result of the shot</returns>
    public abstract ShotResult HandleFire(GameSession context, Position pos);

    /// <summary>
    /// Handles ending the current turn.
    /// </summary>
    /// <param name="context">The game session context</param>
    public abstract void HandleEndTurn(GameSession context);

    /// <summary>
    /// Handles surrendering the game.
    /// </summary>
    /// <param name="context">The game session context</param>
    /// <param name="who">The player who surrenders</param>
    public abstract void HandleSurrender(GameSession context, Player who);

    /// <summary>
    /// Handles proposing a draw.
    /// </summary>
    /// <param name="context">The game session context</param>
    /// <param name="who">The player proposing the draw</param>
    public abstract void HandleProposeDraw(GameSession context, Player who);

    /// <summary>
    /// Handles accepting a draw.
    /// </summary>
    /// <param name="context">The game session context</param>
    /// <param name="who">The player accepting the draw</param>
    public abstract void HandleAcceptDraw(GameSession context, Player who);

    /// <summary>
    /// Handles resetting the boards.
    /// </summary>
    /// <param name="context">The game session context</param>
    public abstract void HandleResetBoards(GameSession context);
}


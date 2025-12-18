namespace BattleShips.Domain.State;

/// <summary>
/// Concrete state representing the finished phase.
/// The game has ended (either by victory, surrender, or draw).
/// </summary>
public sealed class FinishedState : IGameState
{
    public override Phase Phase => Phase.Finished;

    public override bool HandleStart(GameSession context)
    {
        // Cannot start from finished state
        return false;
    }

    public override ShotResult HandleFire(GameSession context, Position pos)
    {
        // Cannot fire after game is finished
        return ShotResult.Invalid;
    }

    public override void HandleEndTurn(GameSession context)
    {
        // No turns after game is finished
    }

    public override void HandleSurrender(GameSession context, Player who)
    {
        // Already finished
    }

    public override void HandleProposeDraw(GameSession context, Player who)
    {
        // Cannot propose draw after game is finished
    }

    public override void HandleAcceptDraw(GameSession context, Player who)
    {
        // Cannot accept draw after game is finished
    }

    public override void HandleResetBoards(GameSession context)
    {
        context.P1.Board.Clear();
        context.P2.Board.Clear();
        context.SetWinner(null);
        context.SetDraw(DrawState.None);
        context.SetCurrent(context.P1);
        // Transition back to Preparation state
        context.TransitionToState(new PreparationState());
    }
}


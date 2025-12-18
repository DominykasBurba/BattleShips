namespace BattleShips.Domain.State;

/// <summary>
/// Concrete state representing the playing phase.
/// The game is in progress and players are taking turns.
/// </summary>
public sealed class PlayingState : IGameState
{
    public override Phase Phase => Phase.Playing;

    public override bool HandleStart(GameSession context)
    {
        // Already started
        return false;
    }

    public override ShotResult HandleFire(GameSession context, Position pos)
    {
        var result = context.Opponent.Board.FireAt(pos);

        if (context.Opponent.Board.AllShipsSunk)
        {
            // Transition to Finished state
            context.TransitionToState(new FinishedState());
            context.SetWinner(context.Current);
        }
        return result;
    }

    public override void HandleEndTurn(GameSession context)
    {
        context.SetCurrent(context.Opponent);
        // Stay in Playing state
    }

    public override void HandleSurrender(GameSession context, Player who)
    {
        // Transition to Finished state
        context.TransitionToState(new FinishedState());
        context.SetWinner(who == context.P1 ? context.P2 : context.P1);
    }

    public override void HandleProposeDraw(GameSession context, Player who)
    {
        var drawState = who == context.P1 ? DrawState.ProposedByP1 : DrawState.ProposedByP2;
        context.SetDraw(drawState);
        // Stay in Playing state
    }

    public override void HandleAcceptDraw(GameSession context, Player who)
    {
        if ((context.Draw == DrawState.ProposedByP1 && who == context.P2) ||
            (context.Draw == DrawState.ProposedByP2 && who == context.P1))
        {
            // Transition to Finished state
            context.TransitionToState(new FinishedState());
            context.SetWinner(null);
            context.SetDraw(DrawState.Accepted);
        }
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


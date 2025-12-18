namespace BattleShips.Domain.State;

/// <summary>
/// Concrete state representing the preparation phase.
/// Players are placing their ships on the board.
/// </summary>
public sealed class PreparationState : IGameState
{
    public override Phase Phase => Phase.Preparation;

    public override bool HandleStart(GameSession context)
    {
        if (!HasCompleteFleet(context.P1) || !HasCompleteFleet(context.P2))
            return false;

        // Transition to Playing state
        context.TransitionToState(new PlayingState());
        return true;
    }

    public override ShotResult HandleFire(GameSession context, Position pos)
    {
        // Cannot fire during preparation
        return ShotResult.Invalid;
    }

    public override void HandleEndTurn(GameSession context)
    {
        // No turns during preparation
    }

    public override void HandleSurrender(GameSession context, Player who)
    {
        // Cannot surrender during preparation
    }

    public override void HandleProposeDraw(GameSession context, Player who)
    {
        // Cannot propose draw during preparation
    }

    public override void HandleAcceptDraw(GameSession context, Player who)
    {
        // Cannot accept draw during preparation
    }

    public override void HandleResetBoards(GameSession context)
    {
        context.P1.Board.Clear();
        context.P2.Board.Clear();
        context.SetWinner(null);
        context.SetDraw(DrawState.None);
        context.SetCurrent(context.P1);
        // Stay in Preparation state
    }

    private static bool HasCompleteFleet(Player p)
    {
        var expectedComposition = Ships.DefaultFleet.Composition
            .GroupBy(k => k)
            .OrderBy(g => g.Key)
            .Select(g => (Kind: g.Key, Count: g.Count()))
            .ToArray();

        var actualComposition = p.Board.Ships
            .GroupBy(s => s.Kind)
            .OrderBy(g => g.Key)
            .Select(g => (Kind: g.Key, Count: g.Count()))
            .ToArray();

        return expectedComposition.SequenceEqual(actualComposition);
    }
}


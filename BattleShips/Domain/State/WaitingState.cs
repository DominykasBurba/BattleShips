namespace BattleShips.Domain.State;

/// <summary>
/// Concrete state representing the waiting phase.
/// Used when waiting for both players to be ready (e.g., in online games).
/// This is the 4th state required by the pattern specification.
/// </summary>
public sealed class WaitingState : IGameState
{
    public override Phase Phase => Phase.Preparation; // Maps to Preparation for compatibility

    public override bool HandleStart(GameSession context)
    {
        // Check if both players are ready
        if (!HasCompleteFleet(context.P1) || !HasCompleteFleet(context.P2))
            return false;

        // Transition to Playing state
        context.TransitionToState(new PlayingState());
        return true;
    }

    public override ShotResult HandleFire(GameSession context, Position pos)
    {
        // Cannot fire while waiting
        return ShotResult.Invalid;
    }

    public override void HandleEndTurn(GameSession context)
    {
        // No turns while waiting
    }

    public override void HandleSurrender(GameSession context, Player who)
    {
        // Cannot surrender while waiting
    }

    public override void HandleProposeDraw(GameSession context, Player who)
    {
        // Cannot propose draw while waiting
    }

    public override void HandleAcceptDraw(GameSession context, Player who)
    {
        // Cannot accept draw while waiting
    }

    public override void HandleResetBoards(GameSession context)
    {
        context.P1.Board.Clear();
        context.P2.Board.Clear();
        context.SetWinner(null);
        context.SetDraw(DrawState.None);
        context.SetCurrent(context.P1);
        // Transition to Preparation state
        context.TransitionToState(new PreparationState());
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


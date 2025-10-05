namespace BattleShips.Domain;

public class GameSession
{
    public Player P1 { get; }
    public Player P2 { get; }
    public Phase Phase { get; private set; } = Phase.Preparation;
    public Player Current { get; private set; }
    public Player Opponent => Current == P1 ? P2 : P1;
    public Player? Winner { get; private set; }
    public DrawState Draw { get; private set; } = DrawState.None;

    public int ShotsPerTurn { get; private set; } = 1;

    public GameSession(Player p1, Player p2)
    {
        P1 = p1; P2 = p2; Current = P1;
    }

    public bool TryStart()
    {
        if (!HasCompleteFleet(P1) || !HasCompleteFleet(P2))
            return false;
        Phase = Phase.Playing;
        return true;
    }

    public void ResetBoards()
    {
        P1.Board.Clear();
        P2.Board.Clear();
        Phase = Phase.Preparation;
        Winner = null;
        Draw = DrawState.None;
        Current = P1;
    }

    public void SetShotsPerTurn(int n) => ShotsPerTurn = Math.Clamp(n, 1, 2);

    public ShotResult Fire(Position pos)
    {
        if (Phase != Phase.Playing) return ShotResult.Invalid;
        var result = Opponent.Board.FireAt(pos);

        if (Opponent.Board.AllShipsSunk)
        {
            Phase = Phase.Finished;
            Winner = Current;
        }
        return result;
    }

    public void EndTurn()
    {
        if (Phase != Phase.Playing) return;
        Current = Opponent;
    }

    public void Surrender(Player who)
    {
        if (Phase == Phase.Finished) return;
        Phase = Phase.Finished;
        Winner = who == P1 ? P2 : P1;
    }

    public void ProposeDraw(Player who)
    {
        if (Phase != Phase.Playing) return;
        Draw = who == P1 ? DrawState.ProposedByP1 : DrawState.ProposedByP2;
    }

    public void AcceptDraw(Player who)
    {
        if (Phase != Phase.Playing) return;
        if ((Draw == DrawState.ProposedByP1 && who == P2) ||
            (Draw == DrawState.ProposedByP2 && who == P1))
        {
            Phase = Phase.Finished;
            Winner = null;
            Draw = DrawState.Accepted;
        }
    }

    private static bool HasCompleteFleet(Player p)
    {
        // 1×4, 2×3, 3×2, 4×1
        var expected = new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 }.OrderBy(x => x).ToArray();
        var actual = p.Board.Ships.Select(s => s.Length).OrderBy(x => x).ToArray();
        return expected.SequenceEqual(actual);
    }
}

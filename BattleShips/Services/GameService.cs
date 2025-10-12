using BattleShips.Domain;

namespace BattleShips.Services;

public class GameService
{
    public GameSession? Session { get; private set; }
    private readonly PlacementService _placement;

    // RULE: when true, a player keeps the turn after Hit/Sunk; turn changes only on Miss/Invalid/AlreadyTried
    public bool KeepTurnOnHit { get; set; } = true;

    // Shooting mode configuration
    public ShootingMode ShootingMode { get; set; } = ShootingMode.Single;

    // still supported for "salvo" style; ignored when KeepTurnOnHit = true
    private int _shotsUsedThisTurn = 0;

    public GameService(PlacementService placement) => _placement = placement;

    public void NewLocalSession(int size = 10, bool enemyIsAi = true)
    {
        var p1 = new HumanPlayer("Player 1", size);
        Player p2 = enemyIsAi ? new AiPlayer("Enemy AI", size) : new HumanPlayer("Player 2", size);
        Session = new GameSession(p1, p2);
        _shotsUsedThisTurn = 0;
    }

    public void ResetShips()
    {
        Session?.ResetBoards();
        _shotsUsedThisTurn = 0;
    }

    public void RandomizeFor(Player who)
    {
        if (Session is null) return;
        _placement.RandomizeFleet(who.Board);
    }

    public void SetShotsPerTurn(int n)
    {
        Session?.SetShotsPerTurn(n);
        _shotsUsedThisTurn = 0;
    }

    public void SetShootingMode(ShootingMode mode)
    {
        ShootingMode = mode;
        _shotsUsedThisTurn = 0;
    }

    public bool Start()
    {
        if (Session is null) return false;
        var ok = Session.TryStart();
        if (ok) _shotsUsedThisTurn = 0;
        return ok;
    }

    public ShotResult FireAt(Position pos)
    {
        if (Session is null || Session.Phase != Phase.Playing) return ShotResult.Invalid;

        // Your shot vs CURRENT opponent
        var result = Session.Fire(pos);
        if (Session.Phase == Phase.Finished) return result;

        // CLASSIC RULE: keep turn on hit/sunk, lose turn on miss/invalid/already tried
        bool endTurn = result is ShotResult.Miss or ShotResult.Invalid or ShotResult.AlreadyTried;

        if (endTurn)
        {
            // Hand turn to opponent (possibly AI)
            Session.EndTurn();

            // If AI's turn, handle AI shooting
            if (Session.Phase == Phase.Playing && Session.Current.Kind == PlayerKind.AI)
            {
                HandleAiTurn();
            }
        }

        return result;
    }

    public List<ShotResult> Fire3x3Salvo(Position centerPos)
    {
        if (Session is null || Session.Phase != Phase.Playing) 
            return new List<ShotResult> { ShotResult.Invalid };

        var results = new List<ShotResult>();
        var boardSize = Session.Opponent.Board.Size;
        
        // Fire at 3x3 grid centered on the clicked position
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                var targetRow = centerPos.Row + dr;
                var targetCol = centerPos.Col + dc;
                
                // Skip if out of bounds
                if (targetRow < 0 || targetRow >= boardSize || targetCol < 0 || targetCol >= boardSize)
                    continue;
                
                var targetPos = new Position(targetRow, targetCol);
                var result = Session.Fire(targetPos);
                results.Add(result);
                
                if (Session.Phase == Phase.Finished) break;
            }
            if (Session.Phase == Phase.Finished) break;
        }

        // Note: Turn ending is handled by the caller (HandleAiTurn or OnFireAtEnemy)

        return results;
    }

    public void HandleAiTurn()
    {
        if (Session is null || Session.Phase != Phase.Playing || Session.Current.Kind != PlayerKind.AI)
            return;

        while (Session.Phase == Phase.Playing && Session.Current.Kind == PlayerKind.AI)
        {
            if (ShootingMode == ShootingMode.Salvo3x3)
            {
                // AI uses 3x3 salvo
                var centerTarget = Session.Current.ChooseTarget(Session.P1.Board, new HashSet<Position>());
                var aiResults = Fire3x3Salvo(centerTarget);
                if (Session.Phase == Phase.Finished) break;
                
                // AI loses turn after 3x3 salvo (regardless of hits/misses)
                break;
            }
            else
            {
                // AI uses single shot
                var target = Session.Current.ChooseTarget(Session.P1.Board, new HashSet<Position>());
                var aiResult = Session.Fire(target);
                if (Session.Phase == Phase.Finished) break;

                // AI loses turn on miss/invalid/duplicate
                if (aiResult is ShotResult.Miss or ShotResult.Invalid or ShotResult.AlreadyTried)
                    break;
            }
        }

        // Hand back to you if game still going
        if (Session.Phase == Phase.Playing) Session.EndTurn();
    }

    public void Surrender(Player who) => Session?.Surrender(who);
    public void ProposeDraw(Player who) => Session?.ProposeDraw(who);
    public void AcceptDraw(Player who) => Session?.AcceptDraw(who);
}

using BattleShips.Domain;
using BattleShips.Domain.AttackStrategies;
using BattleShips.Domain.Ships;

namespace BattleShips.Services;

public class GameService
{
    public GameSession? Session { get; private set; }
    private readonly IFleetPlacer _fleetPlacer;

    // RULE: when true, a player keeps the turn after Hit/Sunk; turn changes only on Miss/Invalid/AlreadyTried
    public bool KeepTurnOnHit { get; set; } = true;

    // Shooting mode configuration
    public ShootingMode ShootingMode { get; set; } = ShootingMode.Single;

    // Attack strategy (Strategy pattern)
    private IAttackStrategy _attackStrategy;

    // still supported for "salvo" style; ignored when KeepTurnOnHit = true
    private int _shotsUsedThisTurn = 0;

    public GameService(IFleetPlacer fleetPlacer)
    {
        _fleetPlacer = fleetPlacer;
        _attackStrategy = GetStrategyForMode(ShootingMode);
    }

    private IAttackStrategy GetStrategyForMode(ShootingMode mode)
    {
        return mode switch
        {
            ShootingMode.Salvo3x3 => new Salvo3x3Strategy(),
            ShootingMode.Single => new SingleShotStrategy(),
            _ => new SingleShotStrategy()
        };
    }

    public void NewLocalSession(int size = 10, bool enemyIsAi = true, ShipType shipType = ShipType.Classic, ShipSkin shipSkin = ShipSkin.Default)
    {
        var p1 = new HumanPlayer("Player 1", size);
        Player p2 = enemyIsAi ? new AiPlayer("Enemy AI", size) : new HumanPlayer("Player 2", size);
        Session = new GameSession(p1, p2);
        _shotsUsedThisTurn = 0;

        // Attach observers to the session (Observer pattern)
        if (Session != null)
        {
            _ = new Domain.Observer.GameStateObserver(Session);
            _ = new Domain.Observer.TurnChangeObserver(Session);
            _ = new Domain.Observer.GameEndObserver(Session);
        }

        // Store ship type and skin for use in RandomizeFor via adapter
        _fleetPlacer.SetShipType(shipType);
        _fleetPlacer.SetShipSkin(shipSkin);
    }

    public void ResetShips()
    {
        Session?.ResetBoards();
        _shotsUsedThisTurn = 0;
    }

    public void ClearSession()
    {
        Session = null;
        _shotsUsedThisTurn = 0;
    }

    public void RandomizeFor(Player who)
    {
        if (Session is null) return;
        _fleetPlacer.PlaceFleet(who.Board);
    }

    public void SetShotsPerTurn(int n)
    {
        Session?.SetShotsPerTurn(n);
        _shotsUsedThisTurn = 0;
    }

    public void SetShootingMode(ShootingMode mode)
    {
        ShootingMode = mode;
        _attackStrategy = GetStrategyForMode(mode);
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

        // Use strategy pattern to execute attack
        var attackResult = _attackStrategy.ExecuteAttack(Session, pos);
        
        if (Session.Phase == Phase.Finished) 
            return attackResult.SingleResult ?? ShotResult.Invalid;

        // Handle turn ending based on strategy result
        if (attackResult.ShouldEndTurn)
        {
            // Hand turn to opponent (possibly AI)
            Session.EndTurn();

            // If AI's turn, handle AI shooting
            if (Session.Phase == Phase.Playing && Session.Current.Kind == PlayerKind.AI)
            {
                HandleAiTurn();
            }
        }

        return attackResult.SingleResult ?? ShotResult.Invalid;
    }

    public List<ShotResult> Fire3x3Salvo(Position centerPos)
    {
        if (Session is null || Session.Phase != Phase.Playing) 
            return new List<ShotResult> { ShotResult.Invalid };

        // Use Salvo3x3Strategy for 3x3 attacks
        var salvoStrategy = new Salvo3x3Strategy();
        var attackResult = salvoStrategy.ExecuteAttack(Session, centerPos);

        return attackResult.Results;
    }

    public void HandleAiTurn()
    {
        if (Session is null || Session.Phase != Phase.Playing || Session.Current.Kind != PlayerKind.AI)
            return;

        while (Session.Phase == Phase.Playing && Session.Current.Kind == PlayerKind.AI)
        {
            // Use the current attack strategy
            var target = Session.Current.ChooseTarget(Session.P1.Board, new HashSet<Position>());
            var attackResult = _attackStrategy.ExecuteAttack(Session, target);
            
            if (Session.Phase == Phase.Finished) break;

            // End AI turn based on strategy result
            if (attackResult.ShouldEndTurn)
                break;
        }

        // Hand back to you if game still going
        if (Session.Phase == Phase.Playing) Session.EndTurn();
    }

    public void Surrender(Player who) => Session?.Surrender(who);
    public void ProposeDraw(Player who) => Session?.ProposeDraw(who);
    public void AcceptDraw(Player who) => Session?.AcceptDraw(who);
}

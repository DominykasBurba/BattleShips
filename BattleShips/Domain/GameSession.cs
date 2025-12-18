using BattleShips.Domain.Memento;
using BattleShips.Domain.Observer;
using BattleShips.Domain.Ships;

namespace BattleShips.Domain;

/// <summary>
/// ConcreteSubject in Observer pattern.
/// Manages game state and notifies observers when state changes.
/// Implements Singleton pattern to ensure only one instance exists.
/// </summary>
public class GameSession : Subject
{
    // Singleton instance
    private static GameSession? _instance;
    private static readonly object _lock = new object();

    public Player P1 { get; private set; }
    public Player P2 { get; private set; }
    
    private Phase _phase = Phase.Preparation;
    private Player _current;
    private Player? _winner;
    private DrawState _draw = DrawState.None;

    public Phase Phase
    {
        get => _phase;
        private set
        {
            if (_phase != value)
            {
                _phase = value;
                Notify(); // jeigu pasikeicia _phase, tai darom notify visiems observeriams
            }
        }
    }

    public Player Current
    {
        get => _current;
        private set
        {
            if (!ReferenceEquals(_current, value))
            {
                _current = value;
                Notify(); // jeigu pasikeicia _phase, tai darom notify visiems observeriams
            }
        }
    }

    public Player Opponent => Current == P1 ? P2 : P1;
    
    public Player? Winner
    {
        get => _winner;
        private set
        {
            if (_winner != value)
            {
                _winner = value;
                Notify(); // jeigu pasikeicia _phase, tai darom notify visiems observeriams
            }
        }
    }

    public DrawState Draw
    {
        get => _draw;
        private set
        {
            if (_draw != value)
            {
                _draw = value;
                Notify(); // jeigu pasikeicia _phase, tai darom notify visiems observeriams
            }
        }
    }

    public int ShotsPerTurn { get; private set; } = 1;


    private GameSession(Player p1, Player p2)
    {
        P1 = p1; 
        P2 = p2; 
        _current = P1;
    }

    public static GameSession GetInstance(Player p1, Player p2)
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new GameSession(p1, p2);
                }
            }
        }
        else
        {
            _instance.Initialize(p1, p2);
        }
        return _instance;
    }

    private void Initialize(Player p1, Player p2)
    {
        P1 = p1;
        P2 = p2;
        _current = P1;
        _phase = Phase.Preparation;
        _winner = null;
        _draw = DrawState.None;
        ShotsPerTurn = 1;
    }
    public static void ResetInstance()
    {
        lock (_lock)
        {
            _instance = null;
        }
    }

    public bool TryStart()
    {
        if (!HasCompleteFleet(P1) || !HasCompleteFleet(P2))
            return false;
        Phase = Phase.Playing; // This will trigger Notify()
        return true;
    }

    public void ResetBoards()
    {
        P1.Board.Clear();
        P2.Board.Clear();
        Phase = Phase.Preparation; // This will trigger Notify()
        Winner = null; // This will trigger Notify()
        Draw = DrawState.None; // This will trigger Notify()
        Current = P1; // This will trigger Notify()
    }

    public void SetShotsPerTurn(int n) => ShotsPerTurn = Math.Clamp(n, 1, 2);

    public ShotResult Fire(Position pos)
    {
        if (Phase != Phase.Playing) return ShotResult.Invalid;
        var result = Opponent.Board.FireAt(pos);

        if (Opponent.Board.AllShipsSunk)
        {
            Phase = Phase.Finished; // This will trigger Notify()
            Winner = Current; // This will trigger Notify()
        }
        return result;
    }

    public void EndTurn()
    {
        if (Phase != Phase.Playing) return;
        Current = Opponent; // This will trigger Notify()
    }

    public void Surrender(Player who)
    {
        if (Phase == Phase.Finished) return;
        Phase = Phase.Finished; // This will trigger Notify()
        Winner = who == P1 ? P2 : P1; // This will trigger Notify()
    }

    public void ProposeDraw(Player who)
    {
        if (Phase != Phase.Playing) return;
        Draw = who == P1 ? DrawState.ProposedByP1 : DrawState.ProposedByP2; // This will trigger Notify()
    }

    public void AcceptDraw(Player who)
    {
        if (Phase != Phase.Playing) return;
        if ((Draw == DrawState.ProposedByP1 && who == P2) ||
            (Draw == DrawState.ProposedByP2 && who == P1))
        {
            Phase = Phase.Finished; // This will trigger Notify()
            Winner = null; // This will trigger Notify()
            Draw = DrawState.Accepted; // This will trigger Notify()
        }
    }

    #region Memento pattern

    /// <summary>
    /// Creates a snapshot of the current game state.
    /// </summary>
    public GameSessionMemento CreateMemento()
    {
        var p1State = CreatePlayerState(P1);
        var p2State = CreatePlayerState(P2);

        return new GameSessionMemento
        {
            P1 = p1State,
            P2 = p2State,
            Phase = Phase,
            CurrentPlayerName = Current.Name,
            WinnerName = Winner?.Name,
            Draw = Draw,
            ShotsPerTurn = ShotsPerTurn
        };
    }

    /// <summary>
    /// Restores game state from a memento snapshot.
    /// </summary>
    public void Restore(GameSessionMemento memento)
    {
        RestorePlayerState(P1, memento.P1);
        RestorePlayerState(P2, memento.P2);

        Phase = memento.Phase;
        Current = P1.Name == memento.CurrentPlayerName ? P1 : P2;
        Winner = memento.WinnerName switch
        {
            null => null,
            var name when name == P1.Name => P1,
            var name when name == P2.Name => P2,
            _ => null
        };
        Draw = memento.Draw;
        ShotsPerTurn = memento.ShotsPerTurn;
    }

    private static PlayerStateDto CreatePlayerState(Player p)
    {
        var ships = p.Board.Ships.Select(s => new ShipStateDto
        {
            Kind = s.Kind,
            Start = s.Start,
            Orientation = s.Orientation,
            IsSunk = s.IsSunk
        }).ToList();

        return new PlayerStateDto
        {
            Name = p.Name,
            Kind = p.Kind,
            BoardSize = p.Board.Size,
            Ships = ships
        };
    }

    private static void RestorePlayerState(Player player, PlayerStateDto dto)
    {
        player.Board.Clear();

        foreach (var shipState in dto.Ships)
        {
            var ship = CreateShipFromState(shipState);
            player.Board.Place(ship);

            if (shipState.IsSunk)
            {
                foreach (var pos in ship.Cells())
                {
                    player.Board.FireAt(pos);
                }
            }
        }
    }

    #endregion

    private static IShip CreateShipFromState(ShipStateDto state)
    {
        return state.Kind switch
        {
            ShipKind.Battleship => new Battleship(state.Start, state.Orientation),
            ShipKind.Submarine => new Submarine(state.Start, state.Orientation),
            ShipKind.Destroyer => new Destroyer(state.Start, state.Orientation),
            ShipKind.Cruiser => new Cruiser(state.Start, state.Orientation),
            _ => throw new InvalidOperationException($"Unsupported ShipKind in memento: {state.Kind}")
        };
    }

    private static bool HasCompleteFleet(Player p)
    {
        // Check if fleet has correct composition based on ShipKind (not exact lengths)
        // This supports both Classic and Modern ship families
        var expectedComposition = DefaultFleet.Composition
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

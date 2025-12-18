using BattleShips.Domain.Memento;
using BattleShips.Domain.Observer;
using BattleShips.Domain.Ships;
using BattleShips.Domain.State;
using BattleShips.Domain.Proxy;

namespace BattleShips.Domain;

/// <summary>
/// Context class in the State pattern.
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
    
    // State pattern: current state object
    private IGameState _currentState;
    
    private Player _current;
    private Player? _winner;
    private DrawState _draw = DrawState.None;

    /// <summary>
    /// Gets the current phase (delegated to current state).
    /// </summary>
    public Phase Phase
    {
        get => _currentState.Phase;
    }

    public Player Current
    {
        get => _current;
    }

    public Player Opponent => Current == P1 ? P2 : P1;

    /// <summary>
    /// Gets P2's board through a protection proxy (hides ship positions from P1).
    /// Implements Proxy pattern for controlled access.
    /// </summary>
    public OpponentBoardProxy P2BoardProxy => new OpponentBoardProxy(P2.Board);

    /// <summary>
    /// Gets P1's board through a protection proxy (hides ship positions from P2).
    /// Implements Proxy pattern for controlled access.
    /// </summary>
    public OpponentBoardProxy P1BoardProxy => new OpponentBoardProxy(P1.Board);

    public Player? Winner
    {
        get => _winner;
    }

    public DrawState Draw
    {
        get => _draw;
    }

    /// <summary>
    /// Internal method for states to set the winner.
    /// </summary>
    internal void SetWinner(Player? winner)
    {
        if (_winner != winner)
        {
            _winner = winner;
            Notify();
        }
    }

    /// <summary>
    /// Internal method for states to set the draw state.
    /// </summary>
    internal void SetDraw(DrawState draw)
    {
        if (_draw != draw)
        {
            _draw = draw;
            Notify();
        }
    }

    /// <summary>
    /// Internal method for states to set the current player.
    /// </summary>
    internal void SetCurrent(Player current)
    {
        if (!ReferenceEquals(_current, current))
        {
            _current = current;
            Notify();
        }
    }

    /// <summary>
    /// Transitions to a new state (State pattern).
    /// </summary>
    internal void TransitionToState(IGameState newState)
    {
        var oldPhase = _currentState.Phase;
        _currentState = newState;
        var newPhase = _currentState.Phase;
        
        // Notify observers if phase changed
        if (oldPhase != newPhase)
        {
            Notify();
        }
    }

    public int ShotsPerTurn { get; private set; } = 1;


    private GameSession(Player p1, Player p2)
    {
        P1 = p1; 
        P2 = p2; 
        _current = P1;
        _currentState = new PreparationState(); // Initial state
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
        _currentState = new PreparationState(); // Reset to initial state
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

    /// <summary>
    /// Requests to start the game (delegated to current state).
    /// </summary>
    public bool TryStart()
    {
        return _currentState.HandleStart(this);
    }

    /// <summary>
    /// Requests to reset the boards (delegated to current state).
    /// </summary>
    public void ResetBoards()
    {
        _currentState.HandleResetBoards(this);
    }

    public void SetShotsPerTurn(int n) => ShotsPerTurn = Math.Clamp(n, 1, 2);

    /// <summary>
    /// Requests to fire at a position (delegated to current state).
    /// </summary>
    public ShotResult Fire(Position pos)
    {
        return _currentState.HandleFire(this, pos);
    }

    /// <summary>
    /// Requests to end the current turn (delegated to current state).
    /// </summary>
    public void EndTurn()
    {
        _currentState.HandleEndTurn(this);
    }

    /// <summary>
    /// Requests to surrender (delegated to current state).
    /// </summary>
    public void Surrender(Player who)
    {
        _currentState.HandleSurrender(this, who);
    }

    /// <summary>
    /// Requests to propose a draw (delegated to current state).
    /// </summary>
    public void ProposeDraw(Player who)
    {
        _currentState.HandleProposeDraw(this, who);
    }

    /// <summary>
    /// Requests to accept a draw (delegated to current state).
    /// </summary>
    public void AcceptDraw(Player who)
    {
        _currentState.HandleAcceptDraw(this, who);
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

        // restore simple fields through internal helpers
        TransitionToState(memento.Phase switch
        {
            Phase.Preparation => new PreparationState(),
            Phase.Playing => new PlayingState(),
            Phase.Finished => new FinishedState(),
            _ => new PreparationState()
        });

        SetCurrent(P1.Name == memento.CurrentPlayerName ? P1 : P2);
        SetWinner(memento.WinnerName switch
        {
            null => null,
            var name when name == P1.Name => P1,
            var name when name == P2.Name => P2,
            _ => null
        });
        SetDraw(memento.Draw);
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

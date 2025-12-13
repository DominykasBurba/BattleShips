using BattleShips.Domain.Observer;
using BattleShips.Domain.State;

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

    /// <summary>
    /// Private constructor for Singleton pattern.
    /// Prevents external instantiation of GameSession.
    /// </summary>
    private GameSession(Player p1, Player p2)
    {
        P1 = p1; 
        P2 = p2; 
        _current = P1;
        _currentState = new PreparationState(); // Initial state
    }

    /// <summary>
    /// Gets the Singleton instance of GameSession.
    /// Creates a new instance if it doesn't exist.
    /// </summary>
    /// <param name="p1">First player</param>
    /// <param name="p2">Second player</param>
    /// <returns>The Singleton instance of GameSession</returns>
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

    /// <summary>
    /// Reinitializes the Singleton instance with new players.
    /// </summary>
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

    /// <summary>
    /// Resets the Singleton instance (for testing or cleanup).
    /// </summary>
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
}

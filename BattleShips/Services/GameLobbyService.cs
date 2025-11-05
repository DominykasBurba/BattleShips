using System.Collections.Concurrent;
using BattleShips.Domain;
using BattleShips.Domain.AttackStrategies;
using BattleShips.Domain.BoardBuilder;
using BattleShips.Domain.Observer;
using BattleShips.Domain.Ships.Factories;
using BattleShips.Hubs;

namespace BattleShips.Services;

public class GameLobbyService(PlacementService placementService)
{
    private readonly ConcurrentDictionary<string, OnlineGameSession> _games = new();
    private readonly ConcurrentDictionary<string, string> _connectionToGame = new();
    private readonly PlacementService _placementService = placementService;

    public string CreateGame(string connectionId, int boardSize, ShootingMode shootingMode = ShootingMode.Single, ShipType shipType = ShipType.Classic)
    {
        var gameId = GenerateGameId();
        var session = new OnlineGameSession(gameId, connectionId, boardSize, shootingMode, shipType);
        _games[gameId] = session;
        _connectionToGame[connectionId] = gameId;
        return gameId;
    }

    public (bool Success, int BoardSize, ShootingMode ShootingMode, ShipType ShipType) JoinGame(string gameId, string connectionId)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return (false, 0, ShootingMode.Single, ShipType.Classic);

        if (session.Player2ConnectionId != null)
            return (false, 0, ShootingMode.Single, ShipType.Classic); // Game is full

        session.Player2ConnectionId = connectionId;
        _connectionToGame[connectionId] = gameId;
        return (true, session.GameSession.P1.Board.Size, session.ShootingMode, session.ShipType);
    }

    public bool PlaceShips(string gameId, string connectionId, List<ShipPlacement> ships)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return false;

        var player = session.GetPlayer(connectionId);
        if (player == null)
            return false;

        // Create the appropriate factory based on ship type
        IShipFactory factory = session.ShipType switch
        {
            ShipType.Modern => new ModernShipFactory(),
            _ => new ClassicShipFactory()
        };

        // Use Builder pattern: ManualFleetBuilder for manual ship placement
        var builder = new ManualFleetBuilder(player.Board, factory);

        // Place each ship using the builder
        foreach (var placement in ships)
        {
            var orientation = placement.IsHorizontal ? Orientation.Horizontal : Orientation.Vertical;
            if (!builder.TryBuildPart(placement.Kind, placement.Start, orientation))
                return false; // Failed to place a ship
        }

        // Mark player as ready
        if (ReferenceEquals(player, session.GameSession.P1))
            session.Player1Ready = true;
        else
            session.Player2Ready = true;

        return true;
    }

    public bool AreBothPlayersReady(string gameId)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return false;

        return session.Player1Ready && session.Player2Ready;
    }

    public bool StartGame(string gameId)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return false;

        if (!session.Player1Ready || !session.Player2Ready)
            return false;

        return session.GameSession.TryStart();
    }

    public string? GetCurrentPlayer(string gameId)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return null;

        return session.GetConnectionId(session.GameSession.Current);
    }

    public (ShotResult Result, Phase GamePhase, string? Winner, string? NextPlayer, List<Position>? SunkShipPositions)? FireShot(
        string gameId, string connectionId, Position position)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return null;

        var player = session.GetPlayer(connectionId);
        if (player == null || session.GameSession.Current != player)
            return null;

        // Get the cell before firing to check if it's a ship
        var targetBoard = session.GameSession.Opponent.Board;
        var cell = targetBoard[position];
        var ship = cell.Ship;

        // Use strategy pattern to execute attack
        var strategy = GetStrategyForMode(session.ShootingMode);
        var attackResult = strategy.ExecuteAttack(session.GameSession, position);
        var result = attackResult.SingleResult ?? ShotResult.Invalid;

        // If ship was sunk, get all its positions
        List<Position>? sunkShipPositions = null;
        if (result == ShotResult.Sunk && ship != null)
        {
            sunkShipPositions = ship.Cells().ToList();
        }

        // Determine if turn should end based on strategy result
        bool endTurn = attackResult.ShouldEndTurn;

        if (endTurn)
        {
            session.GameSession.EndTurn();
        }

        var winner = session.GameSession.Winner?.Name;
        var nextPlayer = session.GetConnectionId(session.GameSession.Current);

        return (result, session.GameSession.Phase, winner, nextPlayer, sunkShipPositions);
    }

    public (List<ShotResult> Results, List<Position> Positions, Phase GamePhase, string? Winner, string? NextPlayer, List<List<Position>> SunkShips)? Fire3x3Salvo(
        string gameId, string connectionId, Position centerPosition)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return null;

        var player = session.GetPlayer(connectionId);
        if (player == null || session.GameSession.Current != player)
            return null;

        // Use Salvo3x3Strategy for 3x3 attacks
        var strategy = new Salvo3x3Strategy();
        var attackResult = strategy.ExecuteAttack(session.GameSession, centerPosition);

        // Get sunk ships information
        var sunkShipsSet = new HashSet<ShipBase>();
        var targetBoard = session.GameSession.Opponent.Board;
        
        for (int i = 0; i < attackResult.Positions.Count; i++)
        {
            var pos = attackResult.Positions[i];
            var result = attackResult.Results[i];
            
            if (result == ShotResult.Sunk)
            {
                var cell = targetBoard[pos];
                var ship = cell.Ship;
                if (ship != null)
                {
                    sunkShipsSet.Add(ship);
                }
            }
        }

        // Convert sunk ships to list of position lists
        var sunkShips = sunkShipsSet.Select(ship => ship.Cells().ToList()).ToList();

        // End turn after salvo (strategy indicates this)
        if (attackResult.ShouldEndTurn && session.GameSession.Phase == Phase.Playing)
        {
            session.GameSession.EndTurn();
        }

        var winner = session.GameSession.Winner?.Name;
        var nextPlayer = session.GetConnectionId(session.GameSession.Current);

        return (attackResult.Results, attackResult.Positions, session.GameSession.Phase, winner, nextPlayer, sunkShips);
    }

    public void Surrender(string gameId, string connectionId)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return;

        var player = session.GetPlayer(connectionId);
        if (player != null)
        {
            session.GameSession.Surrender(player);
        }
    }

    public void ProposeDraw(string gameId, string connectionId)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return;

        var player = session.GetPlayer(connectionId);
        if (player != null)
        {
            session.GameSession.ProposeDraw(player);
        }
    }

    public bool AcceptDraw(string gameId, string connectionId)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return false;

        var player = session.GetPlayer(connectionId);
        if (player != null)
        {
            var drawStateBefore = session.GameSession.Draw;
            session.GameSession.AcceptDraw(player);
            return session.GameSession.Draw == DrawState.Accepted && drawStateBefore != DrawState.Accepted;
        }

        return false;
    }

    public string? GetGameIdForConnection(string connectionId)
    {
        _connectionToGame.TryGetValue(connectionId, out var gameId);
        return gameId;
    }

    public void RemovePlayer(string connectionId)
    {
        if (_connectionToGame.TryRemove(connectionId, out var gameId))
        {
            if (_games.TryGetValue(gameId, out var session))
            {
                // Optionally remove the game if both players disconnect
                // For now, we'll keep the game but mark it as abandoned
            }
        }
    }

    private string GenerateGameId()
    {
        return Guid.NewGuid().ToString("N")[..8].ToUpper();
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
}

public class OnlineGameSession
{
    public string GameId { get; }
    public string Player1ConnectionId { get; }
    public string? Player2ConnectionId { get; set; }
    public GameSession GameSession { get; }
    public bool Player1Ready { get; set; }
    public bool Player2Ready { get; set; }
    public ShootingMode ShootingMode { get; set; }
    public ShipType ShipType { get; set; }
    public int ShotsUsedThisTurn { get; set; }

    public OnlineGameSession(string gameId, string player1ConnectionId, int boardSize, ShootingMode shootingMode = ShootingMode.Single, ShipType shipType = ShipType.Classic)
    {
        GameId = gameId;
        Player1ConnectionId = player1ConnectionId;
        ShootingMode = shootingMode;
        ShipType = shipType;
        ShotsUsedThisTurn = 0;
        var p1 = new HumanPlayer("Player 1", boardSize);
        var p2 = new HumanPlayer("Player 2", boardSize);
        GameSession = new GameSession(p1, p2);

        // Attach observers to the session (Observer pattern)
        _ = new GameStateObserver(GameSession);
        _ = new TurnChangeObserver(GameSession);
        _ = new GameEndObserver(GameSession);
    }

    public Player? GetPlayer(string connectionId)
    {
        if (connectionId == Player1ConnectionId)
            return GameSession.P1;
        if (connectionId == Player2ConnectionId)
            return GameSession.P2;
        return null;
    }

    public string? GetConnectionId(Player player)
    {
        if (ReferenceEquals(player, GameSession.P1))
            return Player1ConnectionId;
        if (ReferenceEquals(player, GameSession.P2))
            return Player2ConnectionId;
        return null;
    }
}

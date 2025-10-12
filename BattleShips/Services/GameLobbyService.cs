using System.Collections.Concurrent;
using BattleShips.Domain;
using BattleShips.Hubs;

namespace BattleShips.Services;

public class GameLobbyService(PlacementService placementService)
{
    private readonly ConcurrentDictionary<string, OnlineGameSession> _games = new();
    private readonly ConcurrentDictionary<string, string> _connectionToGame = new();
    private readonly PlacementService _placementService = placementService;

    public string CreateGame(string connectionId, int boardSize, ShootingMode shootingMode = ShootingMode.Single)
    {
        var gameId = GenerateGameId();
        var session = new OnlineGameSession(gameId, connectionId, boardSize, shootingMode);
        _games[gameId] = session;
        _connectionToGame[connectionId] = gameId;
        return gameId;
    }

    public (bool Success, int BoardSize, ShootingMode ShootingMode) JoinGame(string gameId, string connectionId)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return (false, 0, ShootingMode.Single);

        if (session.Player2ConnectionId != null)
            return (false, 0, ShootingMode.Single); // Game is full

        session.Player2ConnectionId = connectionId;
        _connectionToGame[connectionId] = gameId;
        return (true, session.GameSession.P1.Board.Size, session.ShootingMode);
    }

    public bool PlaceShips(string gameId, string connectionId, List<ShipPlacement> ships)
    {
        if (!_games.TryGetValue(gameId, out var session))
            return false;

        var player = session.GetPlayer(connectionId);
        if (player == null)
            return false;

        // Clear existing ships
        player.Board.Clear();

        // Place ships
        foreach (var placement in ships)
        {
            var orientation = placement.IsHorizontal ? Orientation.Horizontal : Orientation.Vertical;
            var ship = ShipFactory.Create(placement.Kind, placement.Start, orientation);
            player.Board.Place(ship);
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

        var result = session.GameSession.Fire(position);

        // If ship was sunk, get all its positions
        List<Position>? sunkShipPositions = null;
        if (result == ShotResult.Sunk && ship != null)
        {
            sunkShipPositions = ship.Cells().ToList();
        }

        // Increment shot counter
        session.ShotsUsedThisTurn++;

        // Determine if turn should end
        bool endTurn = false;

        if (session.ShootingMode == ShootingMode.Salvo2)
        {
            // For Salvo2, end turn after 2 shots OR on miss/invalid
            if (session.ShotsUsedThisTurn >= 2 || result is ShotResult.Miss or ShotResult.Invalid or ShotResult.AlreadyTried)
            {
                endTurn = true;
            }
        }
        else
        {
            // For single shot mode, end turn on miss/invalid
            if (result is ShotResult.Miss or ShotResult.Invalid or ShotResult.AlreadyTried)
            {
                endTurn = true;
            }
        }

        if (endTurn)
        {
            session.GameSession.EndTurn();
            session.ShotsUsedThisTurn = 0;
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

        var results = new List<ShotResult>();
        var positions = new List<Position>();
        var sunkShipsSet = new HashSet<ShipBase>();
        var boardSize = session.GameSession.Opponent.Board.Size;
        var targetBoard = session.GameSession.Opponent.Board;

        // Fire at 3x3 grid
        for (int dr = -1; dr <= 1; dr++)
        {
            for (int dc = -1; dc <= 1; dc++)
            {
                var targetRow = centerPosition.Row + dr;
                var targetCol = centerPosition.Col + dc;

                if (targetRow < 0 || targetRow >= boardSize || targetCol < 0 || targetCol >= boardSize)
                    continue;

                var targetPos = new Position(targetRow, targetCol);
                var cell = targetBoard[targetPos];
                var ship = cell.Ship;

                var result = session.GameSession.Fire(targetPos);
                results.Add(result);
                positions.Add(targetPos);

                // Track sunk ships (only add each ship once)
                if (result == ShotResult.Sunk && ship != null)
                {
                    sunkShipsSet.Add(ship);
                }

                if (session.GameSession.Phase == Phase.Finished) break;
            }
            if (session.GameSession.Phase == Phase.Finished) break;
        }

        // Convert sunk ships to list of position lists
        var sunkShips = sunkShipsSet.Select(ship => ship.Cells().ToList()).ToList();

        // End turn after salvo
        if (session.GameSession.Phase == Phase.Playing)
        {
            session.GameSession.EndTurn();
        }

        var winner = session.GameSession.Winner?.Name;
        var nextPlayer = session.GetConnectionId(session.GameSession.Current);

        return (results, positions, session.GameSession.Phase, winner, nextPlayer, sunkShips);
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
    public int ShotsUsedThisTurn { get; set; }

    public OnlineGameSession(string gameId, string player1ConnectionId, int boardSize, ShootingMode shootingMode = ShootingMode.Single)
    {
        GameId = gameId;
        Player1ConnectionId = player1ConnectionId;
        ShootingMode = shootingMode;
        ShotsUsedThisTurn = 0;
        var p1 = new HumanPlayer("Player 1", boardSize);
        var p2 = new HumanPlayer("Player 2", boardSize);
        GameSession = new GameSession(p1, p2);

        // Set shots per turn based on shooting mode
        if (shootingMode == ShootingMode.Salvo2)
        {
            GameSession.SetShotsPerTurn(2);
        }
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

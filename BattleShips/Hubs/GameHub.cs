using Microsoft.AspNetCore.SignalR;
using BattleShips.Domain;
using BattleShips.Services;

namespace BattleShips.Hubs;

public class GameHub(GameLobbyService lobby) : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var gameId = lobby.GetGameIdForConnection(Context.ConnectionId);
        if (gameId != null)
        {
            await Clients.Group(gameId).SendAsync("PlayerDisconnected", Context.ConnectionId);
            lobby.RemovePlayer(Context.ConnectionId);
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task<string> CreateGame(int boardSize, ShootingMode shootingMode)
    {
        var gameId = lobby.CreateGame(Context.ConnectionId, boardSize, shootingMode);
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        return gameId;
    }

    public async Task<JoinGameResult?> JoinGame(string gameId)
    {
        var (success, boardSize, shootingMode) = lobby.JoinGame(gameId, Context.ConnectionId);
        if (!success) return null;

        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

        await Clients.OthersInGroup(gameId).SendAsync("OpponentJoined");
        return new JoinGameResult(success, boardSize, shootingMode);
    }

    public async Task PlaceShips(string gameId, List<ShipPlacement> ships)
    {
        var success = lobby.PlaceShips(gameId, Context.ConnectionId, ships);
        if (success)
        {
            await Clients.OthersInGroup(gameId).SendAsync("OpponentReady");

            if (lobby.AreBothPlayersReady(gameId))
            {
                var started = lobby.StartGame(gameId);
                if (started)
                {
                    var currentPlayer = lobby.GetCurrentPlayer(gameId);
                    await Clients.Group(gameId).SendAsync("GameStarted", currentPlayer);
                }
            }
        }
    }

    public async Task StartGame(string gameId)
    {
        var started = lobby.StartGame(gameId);
        if (started)
        {
            var currentPlayer = lobby.GetCurrentPlayer(gameId);
            await Clients.Group(gameId).SendAsync("GameStarted", currentPlayer);
        }
    }

    public async Task FireShot(string gameId, Position position)
    {
        var result = lobby.FireShot(gameId, Context.ConnectionId, position);
        if (result != null)
        {
            await Clients.Group(gameId).SendAsync("ShotFired", new
            {
                Shooter = Context.ConnectionId,
                Position = position,
                Result = result.Value.Result,
                GamePhase = result.Value.GamePhase,
                Winner = result.Value.Winner,
                NextPlayer = result.Value.NextPlayer,
                SunkShipPositions = result.Value.SunkShipPositions
            });
        }
    }

    public async Task Fire3x3Salvo(string gameId, Position centerPosition)
    {
        var results = lobby.Fire3x3Salvo(gameId, Context.ConnectionId, centerPosition);
        if (results != null)
        {
            await Clients.Group(gameId).SendAsync("SalvoFired", new
            {
                Shooter = Context.ConnectionId,
                CenterPosition = centerPosition,
                Results = results.Value.Results,
                Positions = results.Value.Positions,
                GamePhase = results.Value.GamePhase,
                Winner = results.Value.Winner,
                NextPlayer = results.Value.NextPlayer,
                SunkShips = results.Value.SunkShips
            });
        }
    }

    public async Task Surrender(string gameId)
    {
        lobby.Surrender(gameId, Context.ConnectionId);
        await Clients.Group(gameId).SendAsync("PlayerSurrendered", Context.ConnectionId);
    }

    public async Task ProposeDraw(string gameId)
    {
        lobby.ProposeDraw(gameId, Context.ConnectionId);
        await Clients.OthersInGroup(gameId).SendAsync("DrawProposed");
    }

    public async Task AcceptDraw(string gameId)
    {
        var accepted = lobby.AcceptDraw(gameId, Context.ConnectionId);
        if (accepted)
        {
            await Clients.Group(gameId).SendAsync("DrawAccepted");
        }
    }

    public async Task SendMessage(string gameId, string message)
    {
        await Clients.Group(gameId).SendAsync("ReceiveMessage", Context.ConnectionId, message);
    }
}

public record ShipPlacement(ShipKind Kind, Position Start, bool IsHorizontal);
public record JoinGameResult(bool Success, int BoardSize, ShootingMode ShootingMode);

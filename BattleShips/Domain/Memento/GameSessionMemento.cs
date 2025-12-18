namespace BattleShips.Domain.Memento;

/// <summary>
/// Lightweight snapshot of the game session state (Memento pattern).
/// Only holds data, no behavior.
/// </summary>
public sealed class GameSessionMemento
{
    public required PlayerStateDto P1 { get; init; }
    public required PlayerStateDto P2 { get; init; }

    public required Phase Phase { get; init; }
    public required string CurrentPlayerName { get; init; }
    public string? WinnerName { get; init; }
    public required DrawState Draw { get; init; }
    public required int ShotsPerTurn { get; init; }
}

/// <summary>
/// Serializable subset of player state.
/// </summary>
public sealed class PlayerStateDto
{
    public required string Name { get; init; }
    public required PlayerKind Kind { get; init; }
    public required int BoardSize { get; init; }
    public required List<ShipStateDto> Ships { get; init; }
}

public sealed class ShipStateDto
{
    public required ShipKind Kind { get; init; }
    public required Position Start { get; init; }
    public required Orientation Orientation { get; init; }
    public required bool IsSunk { get; init; }
}




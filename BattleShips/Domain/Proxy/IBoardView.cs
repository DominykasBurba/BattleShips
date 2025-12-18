using BattleShips.Domain.Cells;
using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Proxy;

/// <summary>
/// Common interface for Board and OpponentBoardProxy (Proxy pattern).
/// Defines the contract that both the real subject (Board) and proxy implement.
/// </summary>
public interface IBoardView
{
    int Size { get; }
    Cell this[int row, int col] { get; }
    Cell this[Position pos] { get; }
    ShotResult FireAt(Position pos);
    bool AllShipsSunk { get; }
    IReadOnlyList<IShip> Ships { get; }
}

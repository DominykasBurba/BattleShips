using BattleShips.Domain.Cells;
using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Proxy;

/// <summary>
/// Protection Proxy pattern implementation.
/// Controls access to opponent's board, hiding ship positions while allowing attacks.
/// This prevents "cheating" by viewing opponent's ship placements.
///
/// USAGE EXAMPLE:
/// <code>
/// // Player 1 has their board
/// Board player1Board = new Board();
/// // ... player1 places ships ...
///
/// // Player 2 should not see player1's ship positions
/// // Use proxy to provide controlled access
/// OpponentBoardProxy opponentView = new OpponentBoardProxy(player1Board);
///
/// // Player 2 can attack
/// var result = opponentView.FireAt(new Position(3, 4));
///
/// // Player 2 can see revealed cells (hits/misses)
/// var cell = opponentView.GetCell(3, 4); // Shows hit/miss/sunk
///
/// // But unrevealed cells hide ship information
/// var hiddenCell = opponentView.GetCell(5, 5); // Shows Empty even if ship is there
///
/// // Can check if all ships sunk
/// if (opponentView.AllShipsSunk)
/// {
///     Console.WriteLine("Victory!");
/// }
/// </code>
/// </summary>
public class OpponentBoardProxy : IBoardView
{
    private readonly Board _realBoard;

    public int Size => _realBoard.Size;

    /// <summary>
    /// Returns only visible ship count (sunk ships), hiding active ships.
    /// </summary>
    public int VisibleShipCount => _realBoard.Ships.Count(s => s.IsSunk);

    /// <summary>
    /// Returns only sunk ships (hides active ships from opponent).
    /// Implements IBoardView.Ships - filters to prevent revealing ship positions.
    /// </summary>
    public IReadOnlyList<IShip> Ships => _realBoard.Ships.Where(s => s.IsSunk).ToList();

    public OpponentBoardProxy(Board realBoard)
    {
        _realBoard = realBoard ?? throw new ArgumentNullException(nameof(realBoard));
    }

    /// <summary>
    /// Indexer for accessing cells - delegates to GetCell.
    /// Provides same interface as Board for compatibility.
    /// </summary>
    public Cell this[int row, int col] => GetCell(row, col);

    /// <summary>
    /// Indexer for accessing cells by Position - delegates to GetCell.
    /// Provides same interface as Board for compatibility.
    /// </summary>
    public Cell this[Position pos] => GetCell(pos);

    /// <summary>
    /// Provides controlled access to cells - hides unrevealed ship positions.
    /// </summary>
    public Cell GetCell(int row, int col)
    {
        var realCell = _realBoard[row, col];

        // If cell is not revealed yet, hide ship information
        if (!realCell.IsRevealed && realCell.Ship != null)
        {
            // Return a sanitized view - cell appears empty until attacked
            return new Cell(realCell.Pos.Row, realCell.Pos.Col)
            {
                Status = CellStatus.Empty
            };
        }

        // Return actual cell if already revealed (hit, miss, sunk, shielded)
        return realCell;
    }

    /// <summary>
    /// Provides controlled access to cells using Position.
    /// </summary>
    public Cell GetCell(Position pos) => GetCell(pos.Row, pos.Col);

    /// <summary>
    /// Allows firing at the board - delegates to real board.
    /// This is the controlled operation we allow opponents to perform.
    /// </summary>
    public ShotResult FireAt(Position pos)
    {
        return _realBoard.FireAt(pos);
    }

    /// <summary>
    /// Returns whether all ships are sunk (game over condition).
    /// This information is safe to expose.
    /// </summary>
    public bool AllShipsSunk => _realBoard.AllShipsSunk;

    /// <summary>
    /// Gets only the ships that have been sunk (visible to opponent).
    /// Active ships remain hidden.
    /// </summary>
    public IEnumerable<IShip> GetSunkShips()
    {
        return _realBoard.Ships.Where(s => s.IsSunk);
    }
}

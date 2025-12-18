using BattleShips.Domain.Cells;
using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Proxy;

public class OpponentBoardProxy(Board realBoard) : IBoardView
{
    private readonly Board _realBoard = realBoard ?? throw new ArgumentNullException(nameof(realBoard));

    public int Size => _realBoard.Size;

    public int VisibleShipCount => _realBoard.Ships.Count(s => s.IsSunk);
    
    public IReadOnlyList<IShip> Ships => _realBoard.Ships.Where(s => s.IsSunk).ToList();
    
    public Cell this[int row, int col] => GetCell(row, col);
    
    public Cell this[Position pos] => GetCell(pos);

    private Cell GetCell(int row, int col)
    {
        var realCell = _realBoard[row, col];

        if (realCell is { IsRevealed: false, Ship: not null })
        {
            return new Cell(realCell.Pos.Row, realCell.Pos.Col)
            {
                Status = CellStatus.Empty
            };
        }

        return realCell;
    }

    private Cell GetCell(Position pos) => GetCell(pos.Row, pos.Col);

    public ShotResult FireAt(Position pos)
    {
        return _realBoard.FireAt(pos);
    }

    public bool AllShipsSunk => _realBoard.AllShipsSunk;

    public IEnumerable<IShip> GetSunkShips()
    {
        return _realBoard.Ships.Where(s => s.IsSunk);
    }
}

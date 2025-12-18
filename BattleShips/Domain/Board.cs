using BattleShips.Domain.Ships;
using BattleShips.Domain.Cells;

namespace BattleShips.Domain;

public class Board
{
    public int Size { get; }
    private readonly Cell[,] _cells;
    private readonly List<IShip> _ships = new();
    private readonly CellFactory _cellFactory;

    public IReadOnlyList<IShip> Ships => _ships;

    public Board(int size = 10, CellFactory? cellFactory = null)
    {
        Size = size;
        _cellFactory = cellFactory ?? new StandardCellFactory();
        _cells = new Cell[size, size];

        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                _cells[r,c] = _cellFactory.CreateCell(r, c);
    }

    public Cell this[int r, int c] => _cells[r,c];
    public Cell this[Position p] => _cells[p.Row, p.Col];

    public void Clear()
    {
        _ships.Clear();
        for (int r = 0; r < Size; r++)
        for (int c = 0; c < Size; c++)
        {
            // Reset to basic cell
            _cells[r, c] = _cellFactory.CreateCell(r, c);
        }
    }

    public bool Place(IShip ship)
    {
        if (!CanPlace(ship)) return false;
        _ships.Add(ship);
        foreach (var p in ship.Cells())
        {
            var cell = this[p];
            cell.Status = CellStatus.Ship;
            cell.Ship = ship;
        }
        return true;
    }

    public void Remove(IShip ship)
    {
        if (!_ships.Remove(ship)) return;
        foreach (var p in ship.Cells())
        {
            var cell = this[p];
            if (cell.Ship == ship)
            {
                cell.Ship = null;
                cell.Status = CellStatus.Empty;
            }
        }
    }

    public bool CanPlace(IShip ship)
    {
        foreach (var p in ship.Cells())
        {
            if (!p.InBounds(Size)) return false;

            var cell = this[p];
            if (cell.Ship is not null) return false;

            // adjacency rule: no touching (including diagonals)
            if (HasAdjacentShip(p)) return false;
        }
        return true;
    }

    private bool HasAdjacentShip(Position p)
    {
        for (int dr = -1; dr <= 1; dr++)
        for (int dc = -1; dc <= 1; dc++)
        {
            var rr = p.Row + dr;
            var cc = p.Col + dc;
            if (rr < 0 || cc < 0 || rr >= Size || cc >= Size) continue;
            if (rr == p.Row && cc == p.Col) continue;

            var neighbor = _cells[rr, cc];
            if (neighbor.Ship is not null) return true;
        }
        return false;
    }

    public ShotResult FireAt(Position p)
    {
        if (!p.InBounds(Size)) return ShotResult.Invalid;
        var cell = this[p];

        // Allow re-shooting a Shielded cell (special case), but block other revealed cells
        if (cell.IsRevealed && cell.Status != CellStatus.Shielded) return ShotResult.AlreadyTried;

        if ((cell.Status == CellStatus.Ship || cell.Status == CellStatus.Shielded) && cell.Ship is not null)
        {
            var ship = cell.Ship;
            var damageApplied = ship.RegisterHit(p);
            if (cell.Status == CellStatus.Ship)
            {
                if (!damageApplied)
                {
                    cell.Status = CellStatus.Shielded; // absorbed by camouflage
                    return ShotResult.Hit;
                }
                cell.Status = CellStatus.Hit;
            }
            else if (cell.Status == CellStatus.Shielded)
            {
                // follow-up shot should apply damage now
                cell.Status = CellStatus.Hit;
            }

            if (!ship.IsSunk) return ShotResult.Hit;

            foreach (var seg in ship.Cells())
                this[seg].Status = CellStatus.Sunk;
            return ShotResult.Sunk;
        }
        cell.Status = CellStatus.Miss;
        return ShotResult.Miss;
    }


    public bool AllShipsSunk => _ships.Count > 0 && _ships.All(s => s.IsSunk);
}

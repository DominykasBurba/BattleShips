using BattleShips.Domain.Ships;
using BattleShips.Domain.Cells;
using BattleShips.Domain.Iterator;
using BattleShips.Domain.Proxy;

namespace BattleShips.Domain;

/// <summary>
/// Concrete aggregate in the Iterator pattern.
/// Represents a game board with cells that can be iterated.
/// Implements IBoardView as the real subject in the Proxy pattern.
/// </summary>
public class Board : IAggregate<Cell>, IBoardView
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

        // Use Factory Method pattern to create cells
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                _cells[r,c] = _cellFactory.CreateCell(r, c);
    }

    public Cell this[int r, int c] => _cells[r,c];
    public Cell this[Position p] => _cells[p.Row, p.Col];

    public void Clear()
    {
        _ships.Clear();
        
        // Use Iterator pattern to iterate through all cells
        IIterator<Cell> iterator = CreateIterator();
        iterator.First();
        
        while (!iterator.IsDone())
        {
            Cell cell = iterator.CurrentItem();
            // Reset to basic cell - need to recreate it at the same position
            var pos = cell.Pos;
            _cells[pos.Row, pos.Col] = _cellFactory.CreateCell(pos.Row, pos.Col);
            iterator.Next();
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


    public bool AllShipsSunk
    {
        get
        {
            if (_ships.Count == 0) return false;
            
            // Use Iterator pattern to iterate through all ships
            ShipCollection shipCollection = new ShipCollection(_ships);
            IIterator<IShip> iterator = shipCollection.CreateIterator();
            
            iterator.First();
            while (!iterator.IsDone())
            {
                IShip ship = iterator.CurrentItem();
                if (!ship.IsSunk)
                {
                    return false;
                }
                iterator.Next();
            }
            
            return true;
        }
    }

    /// <summary>
    /// Creates an iterator for traversing all cells on the board.
    /// Implements IAggregate<Cell> interface (Iterator pattern).
    /// </summary>
    /// <returns>An iterator for the board's cells</returns>
    public IIterator<Cell> CreateIterator()
    {
        return new BoardCellIterator(_cells, Size);
    }

    /// <summary>
    /// Example method demonstrating Iterator pattern usage.
    /// Counts all revealed cells on the board using the iterator.
    /// </summary>
    public int CountRevealedCells()
    {
        IIterator<Cell> iterator = CreateIterator();
        int count = 0;

        iterator.First();
        while (!iterator.IsDone())
        {
            Cell cell = iterator.CurrentItem();
            if (cell.IsRevealed)
            {
                count++;
            }
            iterator.Next();
        }

        return count;
    }

    /// <summary>
    /// Gets the count of active (non-sunk) ships using Iterator pattern.
    /// Demonstrates ShipListIterator usage.
    /// </summary>
    public int GetActiveShipCount()
    {
        if (_ships.Count == 0) return 0;

        // Use Iterator pattern to iterate through all ships
        ShipCollection shipCollection = new ShipCollection(_ships);
        IIterator<IShip> iterator = shipCollection.CreateIterator();
        int count = 0;

        iterator.First();
        while (!iterator.IsDone())
        {
            IShip ship = iterator.CurrentItem();
            if (!ship.IsSunk)
            {
                count++;
            }
            iterator.Next();
        }

        return count;
    }

    /// <summary>
    /// Gets all hit positions from all ships using Iterator pattern.
    /// Demonstrates both ShipListIterator and PositionSetIterator usage.
    /// </summary>
    public HashSet<Position> GetAllHitPositions()
    {
        HashSet<Position> allHits = new HashSet<Position>();

        if (_ships.Count == 0) return allHits;

        // Use Iterator pattern to iterate through all ships
        ShipCollection shipCollection = new ShipCollection(_ships);
        IIterator<IShip> shipIterator = shipCollection.CreateIterator();

        shipIterator.First();
        while (!shipIterator.IsDone())
        {
            IShip ship = shipIterator.CurrentItem();
            
            // For each ship, get its hit positions
            // Since we can't access private _hits directly, we'll collect from cells
            // But for demonstration, let's use the iterator pattern on ship positions
            foreach (var pos in ship.Cells())
            {
                var cell = this[pos];
                if (cell.Status == CellStatus.Hit || cell.Status == CellStatus.Sunk)
                {
                    allHits.Add(pos);
                }
            }
            
            shipIterator.Next();
        }

        // Use PositionSetIterator to iterate through collected hit positions
        if (allHits.Count > 0)
        {
            PositionSet positionSet = new PositionSet(allHits);
            IIterator<Position> positionIterator = positionSet.CreateIterator();
            
            // This demonstrates PositionSetIterator usage
            // In a real scenario, you might process these positions
            positionIterator.First();
            while (!positionIterator.IsDone())
            {
                Position pos = positionIterator.CurrentItem();
                // Could process each hit position here
                positionIterator.Next();
            }
        }

        return allHits;
    }
}

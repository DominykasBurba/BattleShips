namespace BattleShips.Domain.Iterator;

/// <summary>
/// Concrete iterator for iterating through a 2D array of cells (Cell[,]).
/// Iterates row by row, left to right, top to bottom.
/// </summary>
public sealed class BoardCellIterator : IIterator<Cell>
{
    private readonly Cell[,] _cells;
    private readonly int _size;
    private int _currentRow;
    private int _currentCol;

    public BoardCellIterator(Cell[,] cells, int size)
    {
        _cells = cells;
        _size = size;
        _currentRow = 0;
        _currentCol = 0;
    }

    public void First()
    {
        _currentRow = 0;
        _currentCol = 0;
    }

    public void Next()
    {
        _currentCol++;
        if (_currentCol >= _size)
        {
            _currentCol = 0;
            _currentRow++;
        }
    }

    public bool IsDone()
    {
        return _currentRow >= _size;
    }

    public Cell CurrentItem()
    {
        if (IsDone())
            throw new InvalidOperationException("Iterator is done. No current item.");

        return _cells[_currentRow, _currentCol];
    }
}


using BattleShips.Domain;

namespace BattleShips.Domain.Cells;

/// <summary>
/// Concrete Creator class in Factory Method pattern.
/// Implements the factory method to create standard Cell objects.
/// </summary>
public class StandardCellFactory : CellFactory
{
    /// <summary>
    /// Concrete implementation of the factory method.
    /// Creates a standard Cell instance with the given position.
    /// </summary>
    /// <param name="row">Row position of the cell</param>
    /// <param name="col">Column position of the cell</param>
    /// <returns>A new standard Cell instance</returns>
    public override Cell CreateCell(int row, int col)
    {
        return new Cell(row, col);
    }
}


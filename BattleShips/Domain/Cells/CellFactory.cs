using BattleShips.Domain;

namespace BattleShips.Domain.Cells;

/// <summary>
/// Abstract Creator class in Factory Method pattern.
/// Defines the factory method that creates Cell products.
/// </summary>
public abstract class CellFactory
{
    /// <summary>
    /// Factory Method - creates a Cell product.
    /// Subclasses override this method to create different types of cells.
    /// </summary>
    /// <param name="row">Row position of the cell</param>
    /// <param name="col">Column position of the cell</param>
    /// <returns>A new Cell instance</returns>
    public abstract Cell CreateCell(int row, int col);

    /// <summary>
    /// Template method that can use the factory method.
    /// Can be overridden by subclasses for additional behavior.
    /// </summary>
    public virtual Cell CreateCellWithPosition(Position position)
    {
        return CreateCell(position.Row, position.Col);
    }
}



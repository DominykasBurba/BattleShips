using BattleShips.Domain.Cells;
using BattleShips.Domain.Ships;

namespace BattleShips.Domain.Visitor;

/// <summary>
/// Visitor pattern interface.
/// Defines operations that can be performed when visiting board elements.
/// Different concrete visitors implement different analysis/processing logic.
/// </summary>
public interface IBoardVisitor
{
    /// <summary>
    /// Visit a cell on the board.
    /// Called for each cell during board traversal.
    /// </summary>
    void VisitCell(Cell cell);

    /// <summary>
    /// Visit a ship on the board.
    /// Called for each ship during board traversal.
    /// </summary>
    void VisitShip(IShip ship);

    /// <summary>
    /// Called when the visit is complete.
    /// Visitor can perform final calculations/cleanup.
    /// </summary>
    void VisitComplete();
}

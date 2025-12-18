using BattleShips.Domain;

namespace BattleShips.Domain.Cells;


public class StandardCellFactory : CellFactory
{

    public override Cell CreateCell(int row, int col)
    {
        return new Cell(row, col);
    }
}











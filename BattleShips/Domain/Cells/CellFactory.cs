using BattleShips.Domain;

namespace BattleShips.Domain.Cells;

public abstract class CellFactory
{
    public abstract Cell CreateCell(int row, int col);

    public virtual Cell CreateCellWithPosition(Position position)
    {
        return CreateCell(position.Row, position.Col);
    }
}











using BattleShips.Domain.Ships;

namespace BattleShips.Domain;

public class Cell
{
    public Position Pos { get; }
    public CellStatus Status { get; set; } = CellStatus.Empty;
    public IShip? Ship { get; set; }

    public Cell(int r, int c) => Pos = new Position(r, c);
    public bool IsRevealed => Status is CellStatus.Hit or CellStatus.Miss or CellStatus.Sunk or CellStatus.Shielded;
}